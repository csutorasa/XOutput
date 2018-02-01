using SlimDX;
using SlimDX.DirectInput;
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
        public bool HasAxes => joystick.Capabilities.AxesCount > 0;
        public int ButtonCount => joystick.Capabilities.ButtonCount;

        private readonly DeviceInstance deviceInstance;
        private readonly Joystick joystick;
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
        /// Gets the current state of the DPad.
        /// </summary>
        /// <returns></returns>
        public DPadDirection DPad
        {
            get
            {
                JoystickState state = joystick.GetCurrentState();
                switch (state.GetPointOfViewControllers()[0])
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

        /// <summary>
        /// Gets the current state of the inputTpye.
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        public double Get(Enum inputType)
        {
            if (!(inputType is DirectInputTypes))
                throw new ArgumentException();
            if (inputType.IsAxis())
            {
                return (GetAxisValue((DirectInputTypes)inputType - DirectInputTypes.Axis1 + 1)) / (double)ushort.MaxValue;
            }
            if (inputType.IsButton())
            {
                return GetButtonValue((DirectInputTypes)inputType - DirectInputTypes.Button1 + 1) ? 1d : 0d;
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
                Result result = joystick.Poll();
                if (result.IsSuccess)
                {
                    InputChanged?.Invoke();
                }
                return result.IsSuccess;
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
            JoystickState state = joystick.GetCurrentState();
            if (axis < 1)
                throw new ArgumentException();
            switch(axis)
            {
                case 1:
                    return state.X;
                case 2:
                    return state.Y;
                case 3:
                    return state.Z;
                case 4:
                    return state.RotationX;
                case 5:
                    return state.RotationY;
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
            JoystickState state = joystick.GetCurrentState();
            if (button < 1)
                throw new ArgumentException();
            return state.GetButtons()[button - 1];
        }

        public IEnumerable<Enum> GetButtons()
        {
            return ((DirectInputTypes[])Enum.GetValues(typeof(DirectInputTypes)))
                .Select(x => (Enum)x)
                .Where(b => b.IsButton())
                .Take(ButtonCount);
        }

        public IEnumerable<Enum> GetAxes()
        {
            return ((DirectInputTypes[])Enum.GetValues(typeof(DirectInputTypes)))
                .Select(x => (Enum)x)
                .Where(b => b.IsAxis());
        }
    }
}
