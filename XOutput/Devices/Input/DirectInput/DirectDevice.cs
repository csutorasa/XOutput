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
        /// </summary>
        public Guid Id => deviceInstance.InstanceGuid;
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
        public IEnumerable<Enum> Buttons => buttons;
        /// <summary>
        /// <para>Implements <see cref="IDevice.Axes"/></para>
        /// </summary>
        public IEnumerable<Enum> Axes => axes;
        /// <summary>
        /// <para>Implements <see cref="IDevice.Sliders"/></para>
        /// </summary>
        public IEnumerable<Enum> Sliders => sliders;
        /// <summary>
        /// <para>Implements <see cref="IInputDevice.ForceFeedbackCount"/></para>
        /// </summary>
        public int ForceFeedbackCount => actuators.Count;

        public string HardwareID
        {
            get
            {
                if (deviceInstance.IsHumanInterfaceDevice)
                {
                    string path = joystick.Properties.InterfacePath;
                    if (path.Contains("hid#"))
                    {
                        path = path.Substring(path.IndexOf("hid#"));
                        path = path.Replace('#', '\\');
                        int first = path.IndexOf('\\');
                        int second = path.IndexOf('\\', first + 1);
                        if (second > 0)
                        {
                            return path.Remove(second).ToUpper();
                        }
                    }
                }
                return null;
            }
        }
        #endregion

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(DirectDevice));
        private readonly DeviceInstance deviceInstance;
        private readonly Joystick joystick;
        private readonly Enum[] buttons;
        private readonly Enum[] axes;
        private readonly Enum[] sliders;
        private readonly Enum[] allTypes;
        private readonly DeviceState state;
        private readonly EffectInfo force;
        private readonly Dictionary<DeviceObjectInstance, Effect> actuators;
        private bool connected = false;
        private Thread inputRefresher;
        private bool disposed = false;

        /// <summary>
        /// Creates a new DirectDevice instance.
        /// </summary>
        /// <param name="deviceInstance">SharpDX instanse</param>
        /// <param name="joystick">SharpDX joystick</param>
        public DirectDevice(DeviceInstance deviceInstance, Joystick joystick)
        {
            this.deviceInstance = deviceInstance;
            this.joystick = joystick;
            buttons = DirectInputHelper.Instance.Buttons.Take(joystick.Capabilities.ButtonCount).OfType<Enum>().ToArray();
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
            logger.Info(ToString());
            foreach (var obj in joystick.GetObjects())
            {
                logger.Info("  " + obj.Name + " " + obj.ObjectId + " offset: " + obj.Offset);
            }
            allTypes = buttons.Concat(axes).Concat(sliders).ToArray();
            state = new DeviceState(allTypes, joystick.Capabilities.PovCount);
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
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        public double Get(Enum inputType)
        {
            if (!(inputType is DirectInputTypes))
                throw new ArgumentException();
            var type = (DirectInputTypes)inputType;
            if (DirectInputHelper.Instance.IsAxis(type))
            {
                return (GetAxisValue(type - DirectInputTypes.Axis1 + 1)) / (double)ushort.MaxValue;
            }
            if (DirectInputHelper.Instance.IsButton(type))
            {
                return GetButtonValue(type - DirectInputTypes.Button1 + 1) ? 1d : 0d;
            }
            if (DirectInputHelper.Instance.IsSlider(type))
            {
                return GetSliderValue(type - DirectInputTypes.Slider1 + 1) / (double)ushort.MaxValue;
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
                    joystick.Poll();
                    var newDPads = Enumerable.Range(0, state.DPads.Count()).Select(i => GetDPadValue(i));
                    var newValues = allTypes.ToDictionary(t => t, t => Get(t));
                    var changedDPads = state.SetDPads(newDPads);
                    var changedValues = state.SetValues(newValues);
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
            var state = GetCurrentState();
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
            var state = GetCurrentState();
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
            var state = GetCurrentState();
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
            JoystickState state = GetCurrentState();
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
        private Enum[] GetAxes()
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
                .OrderBy(a => (int)a)
                .OfType<Enum>()
                .ToArray();
        }

        private DirectInputTypes? MapAxisByInstanceNumber(DeviceObjectInstance instance)
        {
            if (joystick.Information.Type != DeviceType.Mouse)
            {
                JoystickOffset offset = (JoystickOffset)instance.Offset;
                if (offset == JoystickOffset.Sliders0 ||
                    offset == JoystickOffset.Sliders1 ||
                    offset == JoystickOffset.VelocitySliders0 ||
                    offset == JoystickOffset.VelocitySliders1 ||
                    offset == JoystickOffset.AccelerationSliders0 ||
                    offset == JoystickOffset.AccelerationSliders1 ||
                    offset == JoystickOffset.ForceSliders0 ||
                    offset == JoystickOffset.ForceSliders1)
                {
                    return null;
                }
            }
            return DirectInputHelper.Instance.Axes.ElementAtOrDefault(instance.ObjectId.InstanceNumber);
        }

        private DirectInputTypes? MapAxisByOffset(DeviceObjectInstance instance)
        {
            if (joystick.Information.Type == DeviceType.Mouse)
            {
                MouseOffset offset = (MouseOffset)instance.Offset;
                switch (offset)
                {
                    case MouseOffset.X: return DirectInputTypes.Axis1;
                    case MouseOffset.Y: return DirectInputTypes.Axis2;
                    case MouseOffset.Z: return DirectInputTypes.Axis3;
                    default: return null;
                }
            }
            else
            {
                JoystickOffset offset = (JoystickOffset)instance.Offset;
                switch (offset)
                {
                    case JoystickOffset.X: return DirectInputTypes.Axis1;
                    case JoystickOffset.Y: return DirectInputTypes.Axis2;
                    case JoystickOffset.Z: return DirectInputTypes.Axis3;
                    case JoystickOffset.RotationX: return DirectInputTypes.Axis4;
                    case JoystickOffset.RotationY: return DirectInputTypes.Axis5;
                    case JoystickOffset.RotationZ: return DirectInputTypes.Axis6;
                    case JoystickOffset.AccelerationX: return DirectInputTypes.Axis7;
                    case JoystickOffset.AccelerationY: return DirectInputTypes.Axis8;
                    case JoystickOffset.AccelerationZ: return DirectInputTypes.Axis9;
                    case JoystickOffset.AngularAccelerationX: return DirectInputTypes.Axis10;
                    case JoystickOffset.AngularAccelerationY: return DirectInputTypes.Axis11;
                    case JoystickOffset.AngularAccelerationZ: return DirectInputTypes.Axis12;
                    case JoystickOffset.ForceX: return DirectInputTypes.Axis13;
                    case JoystickOffset.ForceY: return DirectInputTypes.Axis14;
                    case JoystickOffset.ForceZ: return DirectInputTypes.Axis15;
                    case JoystickOffset.TorqueX: return DirectInputTypes.Axis16;
                    case JoystickOffset.TorqueY: return DirectInputTypes.Axis17;
                    case JoystickOffset.TorqueZ: return DirectInputTypes.Axis18;
                    case JoystickOffset.VelocityX: return DirectInputTypes.Axis19;
                    case JoystickOffset.VelocityY: return DirectInputTypes.Axis20;
                    case JoystickOffset.VelocityZ: return DirectInputTypes.Axis21;
                    case JoystickOffset.AngularVelocityX: return DirectInputTypes.Axis22;
                    case JoystickOffset.AngularVelocityY: return DirectInputTypes.Axis23;
                    case JoystickOffset.AngularVelocityZ: return DirectInputTypes.Axis24;
                    default: return null;
                }
            }
        }

        /// <summary>
        /// Gets available sliders for the device.
        /// </summary>
        /// <returns><see cref="DirectInputTypes"/> of the axes</returns>
        private Enum[] GetSliders()
        {
            int slidersCount = joystick.GetObjects().Where(o => o.ObjectType == ObjectGuid.Slider).Count();
            return DirectInputHelper.Instance.Sliders.Take(slidersCount).OfType<Enum>().ToArray();
        }

        /// <summary>
        /// Reads the current state of the device.
        /// </summary>
        /// <returns>state</returns>
        private JoystickState GetCurrentState()
        {
            try
            {
                return joystick.GetCurrentState();
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
    }
}
