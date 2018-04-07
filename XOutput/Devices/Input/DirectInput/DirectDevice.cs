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
        public event DeviceInputChangedHandler InputChanged;
        public event DeviceDisconnectedHandler Disconnected;
        /// <summary>
        /// The GUID of the controller
        /// </summary>
        public Guid Id => deviceInstance.InstanceGuid;
        public string DisplayName => deviceInstance.ProductName;
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

        public IEnumerable<DPadDirection> DPads => dpads;
        public IEnumerable<Enum> Buttons => buttons;
        public IEnumerable<Enum> Axes => axes;
        public IEnumerable<Enum> Sliders => sliders;
        public int ForceFeedbackCount => actuators.Count;

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(DirectDevice));
        private readonly DeviceInstance deviceInstance;
        private readonly Joystick joystick;
        private readonly Enum[] buttons;
        private readonly Enum[] axes;
        private readonly Enum[] sliders;
        private readonly DPadDirection[] dpads;
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
            dpads = new DPadDirection[joystick.Capabilities.PovCount];

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
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DisplayName + "(" + Id + ")";
        }

        public void StartCapturing()
        {
            if (inputRefresher == null && !disposed)
            {
                inputRefresher = new Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            Connected = RefreshInput();
                            Thread.Sleep(1);
                        }
                    }
                    catch (ThreadAbortException) { }
                });
                inputRefresher.Name = ToString() + " input reader";
                inputRefresher.IsBackground = true;
                Connected = true;
                inputRefresher.Start();
            }
        }

        /// <summary>
        /// Gets the current state of the inputTpye.
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
        /// Refreshes the current state.
        /// </summary>
        /// <returns></returns>
        public bool RefreshInput()
        {
            if (!disposed)
            {
                try
                {
                    joystick.Poll();
                    foreach (var dpad in Enumerable.Range(0, dpads.Length))
                    {
                        dpads[dpad] = GetDPadValue(dpad);
                    }
                    InputChanged?.Invoke(this, new DeviceInputChangedEventArgs());
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

        private Enum[] GetAxes()
        {
            return joystick.GetObjects(DeviceObjectTypeFlags.Axis)
                .Select(MapAxisByInstanceNumber)
                .Where(a => a != null)
                .OrderBy(a => (int)a)
                .OfType<Enum>()
                .ToArray();
        }

        private DirectInputTypes? MapAxisByInstanceNumber(DeviceObjectInstance instance)
        {
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

        private Enum[] GetSliders()
        {
            int slidersCount = joystick.GetObjects().Where(o => o.ObjectType == ObjectGuid.Slider).Count();
            return DirectInputHelper.Instance.Sliders.Take(slidersCount).OfType<Enum>().ToArray();
        }

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

        private int CalculateMagnitude(double value)
        {
            return (int)(10000 * value);
        }
    }
}
