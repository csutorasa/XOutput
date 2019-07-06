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
        /// <para>Implements <see cref="IInputDevice.UniqueId"/></para>
        /// </summary>
        public string UniqueId => deviceInstance.InstanceGuid.ToString();
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
        /// <para>Implements <see cref="IDevice.Sources"/></para>
        /// </summary>
        public IEnumerable<InputSource> Sources => sources;
        /// <summary>
        /// <para>Implements <see cref="IInputDevice.ForceFeedbackCount"/></para>
        /// </summary>
        public int ForceFeedbackCount => actuators.Count;
        /// <summary>
        /// <para>Implements <see cref="IInputDevice.InputConfiguration"/></para>
        /// </summary>
        public InputConfig InputConfiguration => inputConfig;

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
        private readonly DirectInputSource[] sources;
        private readonly DeviceState state;
        private readonly EffectInfo force;
        private readonly Dictionary<DeviceObjectInstance, Effect> actuators;
        private readonly InputConfig inputConfig;
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
            var buttons = joystick.GetObjects(DeviceObjectTypeFlags.Button).Where(b => b.Usage > 0).Take(128).Select(b => new DirectInputSource(this, "Button " + b.Usage, InputSourceTypes.Button, b.Offset, state => state.Buttons[b.ObjectId.InstanceNumber] ? 1 : 0)).ToArray();
            var axes = GetAxes().OrderBy(a => a.Usage).Take(24).Select(GetAxisSource);
            var sliders = GetSliders().OrderBy(a => a.Usage).Select(GetSliderSource);
            IEnumerable<DirectInputSource> dpads = new DirectInputSource[0];
            if (joystick.Capabilities.PovCount > 0)
            {
                dpads = Enumerable.Range(0, joystick.Capabilities.PovCount)
                    .SelectMany(i => new DirectInputSource[] {
                        new DirectInputSource(this, "DPad" + (i + 1) + " Up", InputSourceTypes.Dpad, 1000 + i * 4, state => GetDPadValue(i).HasFlag(DPadDirection.Up) ? 1 : 0),
                        new DirectInputSource(this, "DPad" + (i + 1) + " Down", InputSourceTypes.Dpad, 1001 + i * 4, state => GetDPadValue(i).HasFlag(DPadDirection.Down) ? 1 : 0),
                        new DirectInputSource(this, "DPad" + (i + 1) + " Left", InputSourceTypes.Dpad, 1002 + i * 4, state => GetDPadValue(i).HasFlag(DPadDirection.Left) ? 1 : 0),
                        new DirectInputSource(this, "DPad" + (i + 1) + " Right", InputSourceTypes.Dpad, 1003 + i * 4, state => GetDPadValue(i).HasFlag(DPadDirection.Right) ? 1 : 0),
                    });
            }
            sources = buttons.Concat(axes).Concat(sliders).Concat(dpads).ToArray();

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
                logger.Info("  " + obj.Name + " " + obj.ObjectId + " offset: " + obj.Offset + " objecttype: " + obj.ObjectType.ToString() + " " + obj.Usage);
            }
            state = new DeviceState(sources, joystick.Capabilities.PovCount);
            inputConfig = new InputConfig(ForceFeedbackCount);
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
            return UniqueId;
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
        public double Get(InputSource inputType)
        {
            return inputType.Value;
        }

        /// <summary>
        /// Sets the force feedback motor values.
        /// <para>Implements <see cref="IInputDevice.SetForceFeedback(double, double)"/></para>
        /// </summary>
        /// <param name="big">Big motor value</param>
        /// <param name="small">Small motor value</param>
        public void SetForceFeedback(double big, double small)
        {
            if (ForceFeedbackCount == 0)
            {
                return;
            }
            if (!inputConfig.ForceFeedback)
            {
                big = 0;
                small = 0;
            }
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
        public bool RefreshInput(bool force = false)
        {
            state.ResetChanges();
            if (!disposed)
            {
                try
                {
                    joystick.Poll();
                    for (int i = 0; i < state.DPads.Count(); i++)
                    {
                        state.SetDPad(i, GetDPadValue(i));
                    }
                    foreach (var source in sources)
                    {
                        if (source.Refresh(GetCurrentState()))
                        {
                            state.MarkChanged(source);
                        }
                    }
                    var changes = state.GetChanges(force);
                    var dpadChanges = state.GetChangedDpads();
                    if (changes.Any() || dpadChanges.Any())
                        InputChanged?.Invoke(this, new DeviceInputChangedEventArgs(this, changes, dpadChanges));
                    return true;
                }
                catch (Exception)
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
        private int GetAxisValue(int instanceNumber)
        {
            var state = GetCurrentState();
            if (instanceNumber < 0)
                throw new ArgumentException();
            switch (instanceNumber)
            {
                case 0:
                    return state.X;
                case 1:
                    return ushort.MaxValue - state.Y;
                case 2:
                    return state.Z;
                case 3:
                    return state.RotationX;
                case 4:
                    return ushort.MaxValue - state.RotationY;
                case 5:
                    return state.RotationZ;
                case 6:
                    return state.AccelerationX;
                case 7:
                    return ushort.MaxValue - state.AccelerationY;
                case 8:
                    return state.AccelerationZ;
                case 9:
                    return state.AngularAccelerationX;
                case 10:
                    return ushort.MaxValue - state.AngularAccelerationY;
                case 11:
                    return state.AngularAccelerationZ;
                case 12:
                    return state.ForceX;
                case 13:
                    return ushort.MaxValue - state.ForceY;
                case 14:
                    return state.ForceZ;
                case 15:
                    return state.TorqueX;
                case 16:
                    return ushort.MaxValue - state.TorqueY;
                case 17:
                    return state.TorqueZ;
                case 18:
                    return state.VelocityX;
                case 19:
                    return ushort.MaxValue - state.VelocityY;
                case 20:
                    return state.VelocityZ;
                case 21:
                    return state.AngularVelocityX;
                case 22:
                    return ushort.MaxValue - state.AngularVelocityY;
                case 23:
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
        private DeviceObjectInstance[] GetAxes()
        {
            var axes = joystick.GetObjects(DeviceObjectTypeFlags.AbsoluteAxis).Where(o => o.ObjectType != ObjectGuid.Slider).ToArray();
            foreach (var axis in axes)
            {
                var properties = joystick.GetObjectPropertiesById(axis.ObjectId);
                try
                {
                    properties.Range = new InputRange(ushort.MinValue, ushort.MaxValue);
                    properties.DeadZone = 0;
                    properties.Saturation = 10000;
                }
                catch (SharpDXException ex)
                {
                    logger.Error(ex);
                }
            }
            return axes;
        }

        /// <summary>
        /// Gets available sliders for the device.
        /// </summary>
        /// <returns><see cref="DirectInputTypes"/> of the axes</returns>
        private DeviceObjectInstance[] GetSliders()
        {
            return joystick.GetObjects().Where(o => o.ObjectType == ObjectGuid.Slider).ToArray();
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

        private DirectInputSource GetAxisSource(DeviceObjectInstance instance)
        {
            InputSourceTypes type = InputSourceTypes.AxisX;
            if (instance.ObjectType == ObjectGuid.XAxis || instance.ObjectType == ObjectGuid.RxAxis)
            {
                type = InputSourceTypes.AxisX;
            }
            else if (instance.ObjectType == ObjectGuid.YAxis || instance.ObjectType == ObjectGuid.RyAxis)
            {
                type = InputSourceTypes.AxisY;
            }
            else if (instance.ObjectType == ObjectGuid.ZAxis || instance.ObjectType == ObjectGuid.RzAxis)
            {
                type = InputSourceTypes.AxisZ;
            }
            int axisCount;
            if (instance.Usage >= 48)
            {
                axisCount = instance.Usage - 48;
            }
            else
            {
                axisCount = instance.ObjectId.InstanceNumber;
            }
            string name = instance.Name;
            return new DirectInputSource(this, name, type, instance.Offset, state => (GetAxisValue(axisCount)) / (double)ushort.MaxValue);
        }

        private DirectInputSource GetSliderSource(DeviceObjectInstance instance, int i)
        {
            string name = instance.Name;
            return new DirectInputSource(this, name, InputSourceTypes.Slider, instance.Offset, state => (GetSliderValue(i + 1)) / (double)ushort.MaxValue);
        }
    }
}
