using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Input;

namespace XOutput.Input.DirectInput
{
    /// <summary>
    /// Device that contains data for a DirectInput device
    /// </summary>
    public sealed class DirectDevice : IInputDevice
    {
        public event Action InputChanged;
        /// <summary>
        /// The GUID of the controller
        /// </summary>
        public Guid Id => deviceInstance.ProductGuid;
        public string DisplayName => deviceInstance.ProductName;
        public bool Connected => connected;
        public bool HasDPad => joystick.Capabilities.PovCount > 0;
        public IEnumerable<Enum> Buttons => buttons;
        public IEnumerable<Enum> Axes => axes;
        public IEnumerable<Enum> Sliders => sliders;

        public DPadDirection DPad
        {
            get
            {
                JoystickState state = joystick.GetCurrentState();
                switch (state.PointOfViewControllers[0])
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
        }
        
        private readonly DeviceInstance deviceInstance;
        private readonly Joystick joystick;
        private readonly Enum[] buttons;
        private readonly Enum[] axes;
        private readonly Enum[] sliders;
        private bool connected = false;
        private Thread inputRefresher;
        private bool disposed = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceInstance">SlimDX instanse</param>
        /// <param name="joystick">SlimDX joystick</param>
        public DirectDevice(DeviceInstance deviceInstance, Joystick joystick)
        {
            this.deviceInstance = deviceInstance;
            this.joystick = joystick;
            buttons = DirectInputHelper.Instance.Buttons.Take(joystick.Capabilities.ButtonCount).OfType<Enum>().ToArray();
            axes = getAxes();
            sliders = getSliders();

            joystick.Acquire();
            connected = true;
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
                connected = false;
                if (inputRefresher != null)
                    inputRefresher.Abort();
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
            if(inputRefresher == null && !disposed)
            {
                inputRefresher = new Thread(() => {
                    try
                    {
                        while (true)
                        {
                            connected = RefreshInput();
                            Thread.Sleep(1);
                        }
                    }
                    catch (ThreadAbortException) { }
                });
                inputRefresher.IsBackground = true;
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

        /// <summary>
        /// Refreshes the current state.
        /// </summary>
        /// <returns></returns>
        public bool RefreshInput()
        {
            if (!disposed)
            {
                joystick.Poll();
                InputChanged?.Invoke();
                return true;
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
            var state = joystick.GetCurrentState();
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
            var state = joystick.GetCurrentState();
            if (button < 1)
                throw new ArgumentException();
            return state.Buttons[button - 1];
        }

        /// <summary>
        /// Gets the current value of a slider.
        /// </summary>
        /// <param name="button">Slider index</param>
        /// <returns>Value</returns>
        private int GetSliderValue(int slider)
        {
            var state = joystick.GetCurrentState();
            if (slider < 1)
                throw new ArgumentException();
            return state.Sliders[slider - 1];
        }

        private Enum[] getAxes()
        {
            return joystick.GetObjects(DeviceObjectTypeFlags.Axis)
                .Select(o =>
                {
                    if (o.ObjectType == ObjectGuid.XAxis)
                        return DirectInputTypes.Axis1;
                    if (o.ObjectType == ObjectGuid.YAxis)
                        return DirectInputTypes.Axis2;
                    if (o.ObjectType == ObjectGuid.ZAxis)
                        return DirectInputTypes.Axis3;
                    if (o.ObjectType == ObjectGuid.RxAxis)
                        return DirectInputTypes.Axis4;
                    if (o.ObjectType == ObjectGuid.RyAxis)
                        return DirectInputTypes.Axis5;
                    if (o.ObjectType == ObjectGuid.RzAxis)
                        return DirectInputTypes.Axis6;
                    return (DirectInputTypes?)null;
                })
                .Where(a => a != null)
                .OrderBy(a => (int)a)
                .OfType<Enum>()
                .ToArray();
        }

        private Enum[] getSliders()
        {
            int slidersCount = joystick.GetObjects().Where(o => o.ObjectType == ObjectGuid.Slider).Count();
            return DirectInputHelper.Instance.Sliders.Take(slidersCount).OfType<Enum>().ToArray();
        }
    }
}
