using SharpDX;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using XOutput.Devices.Input.Settings;
using XOutput.Logging;

namespace XOutput.Devices.Input.DirectInput
{
    /// <summary>
    /// Device that contains data for a DirectInput device
    /// </summary>
    public sealed class DirectDevice : IInputDevice
    {
        #region Constants
        /// <summary>
        /// The delay in milliseconds to sleep between input reads.
        /// </summary>
        public const int ReadDelayMs = 1;
        #endregion

        #region Events
        /// <summary>
        /// Triggered periodically to trigger input read from Direct input device.
        /// <para>Implements <see cref="IDevice.InputChanged"/></para>
        /// </summary>
        public event DeviceInputChangedHandler InputChanged;
        /// <summary>
        /// Triggered when the any read or write fails.
        /// <para>Implements <see cref="IInputDevice.Disconnected"/></para>
        /// </summary>
        public event DeviceDisconnectedHandler Disconnected;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the GUID of the controller.
        /// <para>Implements <see cref="IInputDevice.Id"/></para>
        /// </summary>
        public string Id => deviceInstance.InstanceGuid.ToString();
        /// <summary>
        /// Gets the product name of the device.
        /// <para>Implements <see cref="IInputDevice.DisplayName"/></para>
        /// </summary>
        public string DisplayName => deviceInstance.ProductName;
        /// <summary>
        /// Gets or sets if the device is connected and ready to use.
        /// <para>Implements <see cref="IInputDevice.Connected"/></para>
        /// </summary>
        public bool Connected
        {
            get => connected;
            set
            {
                if (value != connected)
                {
                    if (!connected)
                    {
                        Disconnected?.Invoke(this, new DeviceDisconnectedEventArgs());
                    }
                    connected = value;
                }
            }
        }
        /// <summary>
        /// <para>Implements <see cref="IDevice.DPads"/></para>
        /// </summary>
        public IEnumerable<DPadDirection> DPads => state.DPads;
        /// <summary>
        /// <para>Implements <see cref="IDevice.Buttons"/></para>
        /// </summary>
        public IEnumerable<InputType> Buttons => buttons;
        /// <summary>
        /// <para>Implements <see cref="IDevice.Axes"/></para>
        /// </summary>
        public IEnumerable<InputType> Axes => axes;
        /// <summary>
        /// <para>Implements <see cref="IDevice.Sliders"/></para>
        /// </summary>
        public IEnumerable<InputType> Sliders => sliders;
        /// <summary>
        /// <para>Implements <see cref="IDevice.Values"/></para>
        /// </summary>
        public IEnumerable<InputType> Values => allTypes;
        /// <summary>
        /// <para>Implements <see cref="IInputDevice.ForceFeedbackCount"/></para>
        /// </summary>
        public int ForceFeedbackCount => actuators.Count;
        #endregion

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(DirectDevice));
        private readonly DeviceInstance deviceInstance;
        private readonly Joystick joystick;
        private readonly InputType[] buttons;
        private readonly InputType[] axes;
        private readonly InputType[] sliders;
        private readonly InputType[] allTypes;
        private readonly DeviceState state;
        private readonly EffectInfo force;
        private readonly Dictionary<DeviceObjectInstance, Effect> actuators;
        private bool connected = false;
        private Thread inputRefresher;
        private bool disposed = false;
        private JoystickState joystickState = new JoystickState();

        /// <summary>
        /// Creates a new DirectDevice instance.
        /// </summary>
        /// <param name="deviceInstance">SharpDX instanse</param>
        /// <param name="joystick">SharpDX joystick</param>
        public DirectDevice(DeviceInstance deviceInstance, Joystick joystick)
        {
            this.deviceInstance = deviceInstance;
            this.joystick = joystick;
            buttons = Enumerable.Range(0, joystick.Capabilities.ButtonCount).Select(b => new InputType { Type = InputTypes.Button, Count = b + 1 }).ToArray();
            axes = GetAxes();
            sliders = GetSliders();

            joystick.Properties.AxisMode = DeviceAxisMode.Absolute;
            try
            {
                joystick.SetCooperativeLevel(new WindowInteropHelper(Application.Current.MainWindow).Handle, CooperativeLevel.Background | CooperativeLevel.Exclusive);
            }
            catch
            {
                logger.Warning($"Failed to set cooperative level to exclusive for {ToString()}");
            }
            joystick.Acquire();
            if (deviceInstance.ForceFeedbackDriverGuid != Guid.Empty)
            {
                var constantForce = joystick.GetEffects().Where(x => x.Guid == EffectGuid.ConstantForce).FirstOrDefault();
                if (constantForce == null)
                    force = joystick.GetEffects().FirstOrDefault();
                else
                    force = constantForce;
                actuators = joystick.GetObjects().Where(doi => doi.ObjectId.Flags.HasFlag(DeviceObjectTypeFlags.ForceFeedbackActuator)).ToDictionary(doi => doi, doi => (Effect)null);
            }
            else
            {
                actuators = new Dictionary<DeviceObjectInstance, Effect>();
            }
            allTypes = buttons.Concat(axes).Concat(sliders).ToArray();
            state = new DeviceState(allTypes, joystick.Capabilities.PovCount, Get, GetDPadValue);
            inputRefresher = new Thread(InputRefresher);
            inputRefresher.Name = ToString() + " input reader";
            inputRefresher.SetApartmentState(ApartmentState.STA);
            inputRefresher.IsBackground = true;
            Connected = true;
            inputRefresher.Start();
        }

        ~DirectDevice()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes all resources.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                inputRefresher?.Abort();
                joystick.Dispose();
                Disconnected?.Invoke(this, new DeviceDisconnectedEventArgs());
            }
        }

        /// <summary>
        /// Display name followed by the deviceID.
        /// <para>Overrides <see cref="object.ToString()"/></para>
        /// </summary>
        /// <returns>Friendly name</returns>
        public override string ToString()
        {
            return DisplayName + "(" + Id + ")";
        }

        private void InputRefresher()
        {
            try
            {
                while (true)
                {
                    Connected = RefreshInput();
                    Thread.Sleep(ReadDelayMs);
                }
            }
            catch (ThreadAbortException) { }
        }

        /// <summary>
        /// Gets the current state of the inputTpye.
        /// <para>Implements <see cref="IDevice.Get(Enum)"/></para>
        /// </summary>
        /// <param name="type">Type of input</param>
        /// <returns>Value</returns>
        public double Get(InputType type)
        {
            double raw = GetRaw(type);
            if (type.IsAxis())
            {
                Dictionary<InputType, InputSettings> settings = Tools.Settings.Instance.GetInputSettings(Id)?.InputSettings;
                InputSettings setting;
                if (!settings.TryGetValue(type, out setting))
                {
                    return raw;
                }
                if (Math.Abs(raw - 0.5) < setting.Deadzone)
                {
                    return 0.5;
                }
                if (Math.Abs(raw) < setting.AntiDeadzone)
                {
                    return 0;
                }
                if (Math.Abs(1 - raw) < setting.AntiDeadzone)
                {
                    return 1;
                }
            }
            return raw;
        }

        /// <summary>
        /// Gets the current raw state of the inputTpye.
        /// <param>Implements <see cref="IInputDevice.GetRaw(Enum)"/></param>
        /// </summary>
        /// <param name="type">Type of input</param>
        /// <returns>Value</returns>
        public double GetRaw(InputType type)
        {
            if (type.IsAxis())
            {
                return GetAxisValue(type.Count) / (double)ushort.MaxValue;
            }
            if (type.IsButton())
            {
                return GetButtonValue(type.Count) ? 1d : 0d;
            }
            if (type.IsSlider())
            {
                return GetSliderValue(type.Count) / (double)ushort.MaxValue;
            }
            return 0;
        }

        /// <summary>
        /// Sets the force feedback motor values.
        /// <para>Implements <see cref="IInputDevice.SetForceFeedback(double, double)"/></para>
        /// </summary>
        /// <param name="big">Big motor value</param>
        /// <param name="small">Small motor value</param>
        public void SetForceFeedback(double big, double small)
        {
            var values = new Dictionary<DeviceObjectInstance, Effect>(actuators);
            foreach (var pair in values)
            {
                var oldEffect = pair.Value;
                oldEffect?.Dispose();

                var actuator = pair.Key;
                var isSmall = actuator.ObjectType == ObjectGuid.YAxis;
                // All available axes will be added.
                var axes = new List<int> { (int)actuator.ObjectId };
                axes.AddRange(actuators.Keys.Select(a => (int)a.ObjectId).Where(a => a != (int)actuator.ObjectId));
                // If two axes are used only the first one will have direction 1.
                var directions = new int[axes.Count];
                if (directions.Length > 1)
                    directions[0] = 1;

                var effectParams = new EffectParameters();
                effectParams.Flags = EffectFlags.Cartesian | EffectFlags.ObjectIds;
                effectParams.StartDelay = 0;
                effectParams.SamplePeriod = joystick.Capabilities.ForceFeedbackSamplePeriod;
                effectParams.Duration = int.MaxValue;
                effectParams.TriggerButton = -1;
                effectParams.TriggerRepeatInterval = int.MaxValue;
                effectParams.Gain = 10000;
                effectParams.SetAxes(axes.ToArray(), directions);
                var cf = new ConstantForce();
                cf.Magnitude = CalculateMagnitude(isSmall ? small : big);
                effectParams.Parameters = cf;
                try
                {
                    var newEffect = new Effect(joystick, force.Guid, effectParams);
                    newEffect.Start();
                    actuators[actuator] = newEffect;
                }
                catch (SharpDXException)
                {
                    logger.Warning($"Failed to create and start effect for {ToString()}");
                }
            }
        }

        /// <summary>
        /// Refreshes the current state. Triggers <see cref="InputChanged"/> event.
        /// </summary>
        /// <returns>if the input was available</returns>
        public bool RefreshInput()
        {
            if (!disposed)
            {
                try
                {
                    joystickState = GetCurrentState();
                    var changedDPads = state.SetDPads();
                    var changedValues = state.SetValues();
                    if (changedDPads.Any() || changedValues.Any())
                        InputChanged?.Invoke(this, new DeviceInputChangedEventArgs(changedValues, changedDPads));
                    return true;
                }
                catch
                {
                    logger.Warning($"Poll failed for {ToString()}");
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the current value of an axis.
        /// </summary>
        /// <param name="axis">Axis index</param>
        /// <returns>Value</returns>
        private int GetAxisValue(int axis)
        {
            var state = joystickState;
            if (axis < 1)
                throw new ArgumentException();
            switch (axis)
            {
                case 1:
                    return state.X;
                case 2:
                    return ushort.MaxValue - state.Y;
                case 3:
                    return state.Z;
                case 4:
                    return state.RotationX;
                case 5:
                    return ushort.MaxValue - state.RotationY;
                case 6:
                    return state.RotationZ;
                case 7:
                    return state.AccelerationX;
                case 8:
                    return ushort.MaxValue - state.AccelerationY;
                case 9:
                    return state.AccelerationZ;
                case 10:
                    return state.AngularAccelerationX;
                case 11:
                    return ushort.MaxValue - state.AngularAccelerationY;
                case 12:
                    return state.AngularAccelerationZ;
                case 13:
                    return state.ForceX;
                case 14:
                    return ushort.MaxValue - state.ForceY;
                case 15:
                    return state.ForceZ;
                case 16:
                    return state.TorqueX;
                case 17:
                    return ushort.MaxValue - state.TorqueY;
                case 18:
                    return state.TorqueZ;
                case 19:
                    return state.VelocityX;
                case 20:
                    return ushort.MaxValue - state.VelocityY;
                case 21:
                    return state.VelocityZ;
                case 22:
                    return state.AngularVelocityX;
                case 23:
                    return ushort.MaxValue - state.AngularVelocityY;
                case 24:
                    return state.AngularVelocityZ;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets the current value of a button.
        /// </summary>
        /// <param name="button">Button index</param>
        /// <returns>Value</returns>
        private bool GetButtonValue(int button)
        {
            var state = joystickState;
            if (button < 1)
                throw new ArgumentException();
            return state.Buttons[button - 1];
        }

        /// <summary>
        /// Gets the current value of a slider.
        /// </summary>
        /// <param name="slider">Slider index</param>
        /// <returns>Value</returns>
        private int GetSliderValue(int slider)
        {
            var state = joystickState;
            if (slider < 1)
                throw new ArgumentException();
            return state.Sliders[slider - 1];
        }

        /// <summary>
        /// Gets the current value of a DPad.
        /// </summary>
        /// <param name="dpad">DPad index</param>
        /// <returns>Value</returns>
        private DPadDirection GetDPadValue(int dpad)
        {
            JoystickState state = joystickState;
            switch (state.PointOfViewControllers[dpad])
            {
                case -1: return DPadDirection.None;
                case 0: return DPadDirection.Up;
                case 4500: return DPadDirection.Up | DPadDirection.Right;
                case 9000: return DPadDirection.Right;
                case 13500: return DPadDirection.Down | DPadDirection.Right;
                case 18000: return DPadDirection.Down;
                case 22500: return DPadDirection.Down | DPadDirection.Left;
                case 27000: return DPadDirection.Left;
                case 31500: return DPadDirection.Up | DPadDirection.Left;
                default:
                    throw new ArgumentException();
            }
        }

        /// <summary>
        /// Gets and initializes available axes for the device.
        /// </summary>
        /// <returns><see cref="DirectInputTypes"/> of the axes</returns>
        private InputType[] GetAxes()
        {
            var axes = joystick.GetObjects(DeviceObjectTypeFlags.Axis).ToArray();
            foreach (var axis in axes)
            {
                var properties = joystick.GetObjectPropertiesById(axis.ObjectId);
                properties.Range = new InputRange(ushort.MinValue, ushort.MaxValue);
                properties.DeadZone = 0;
                properties.Saturation = 10000;
            }
            return axes
                .Select(MapAxisByInstanceNumber)
                .Where(a => a != null)
                .Select(a => (int)a)
                .OrderBy(a => a)
                .Select(a => new InputType { Type = InputTypes.Axis, Count = a + 1 })
                .ToArray();
        }

        private int? MapAxisByInstanceNumber(DeviceObjectInstance instance)
        {
            return instance.ObjectId.InstanceNumber;
        }

        private int? MapAxisByOffset(DeviceObjectInstance instance)
        {
            if (joystick.Information.Type == DeviceType.Mouse)
            {
                MouseOffset offset = (MouseOffset)instance.Offset;
                switch (offset)
                {
                    case MouseOffset.X: return 1;
                    case MouseOffset.Y: return 2;
                    case MouseOffset.Z: return 3;
                    default: return null;
                }
            }
            else
            {
                JoystickOffset offset = (JoystickOffset)instance.Offset;
                switch (offset)
                {
                    case JoystickOffset.X: return 1;
                    case JoystickOffset.Y: return 2;
                    case JoystickOffset.Z: return 3;
                    case JoystickOffset.RotationX: return 4;
                    case JoystickOffset.RotationY: return 5;
                    case JoystickOffset.RotationZ: return 6;
                    case JoystickOffset.AccelerationX: return 7;
                    case JoystickOffset.AccelerationY: return 8;
                    case JoystickOffset.AccelerationZ: return 9;
                    case JoystickOffset.AngularAccelerationX: return 10;
                    case JoystickOffset.AngularAccelerationY: return 11;
                    case JoystickOffset.AngularAccelerationZ: return 12;
                    case JoystickOffset.ForceX: return 13;
                    case JoystickOffset.ForceY: return 14;
                    case JoystickOffset.ForceZ: return 15;
                    case JoystickOffset.TorqueX: return 16;
                    case JoystickOffset.TorqueY: return 17;
                    case JoystickOffset.TorqueZ: return 18;
                    case JoystickOffset.VelocityX: return 19;
                    case JoystickOffset.VelocityY: return 20;
                    case JoystickOffset.VelocityZ: return 21;
                    case JoystickOffset.AngularVelocityX: return 22;
                    case JoystickOffset.AngularVelocityY: return 23;
                    case JoystickOffset.AngularVelocityZ: return 24;
                    default: return null;
                }
            }
        }

        /// <summary>
        /// Gets available sliders for the device.
        /// </summary>
        /// <returns><see cref="DirectInputTypes"/> of the axes</returns>
        private InputType[] GetSliders()
        {
            int slidersCount = joystick.GetObjects().Where(o => o.ObjectType == ObjectGuid.Slider).Count();
            return Enumerable.Range(0, slidersCount).Select(b => new InputType { Type = InputTypes.Slider, Count = b + 1 }).ToArray();
        }

        /// <summary>
        /// Reads the current state of the device.
        /// </summary>
        /// <returns>state</returns>
        private JoystickState GetCurrentState()
        {
            try
            {
                joystick.Poll();
                joystick.GetCurrentState(ref joystickState);
                return joystickState;
            }
            catch (Exception)
            {
                Connected = false;
                return new JoystickState();
            }
        }

        /// <summary>
        /// Calculates the magnitude value from 0-1 values.
        /// </summary>
        /// <param name="value">ratio</param>
        /// <returns>magnitude value</returns>
        private int CalculateMagnitude(double value)
        {
            return (int)(10000 * value);
        }

        public string ConvertToString(InputType type)
        {
            if (type.IsAxis())
            {
                return "A" + type.Count;
            }
            else if (type.IsButton())
            {
                return "B" + type.Count;
            }
            else if (type.IsSlider())
            {
                return "S" + type.Count;
            }
            return "DISABLED";
        }
    }
}
