using SlimDX;
using SlimDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Input.DirectInput
{
    /// <summary>
    /// Device that contains data for a DirectInput device
    /// </summary>
    public sealed class DirectDevice : IDisposable
    {
        /// <summary>
        /// This event is invoked if the data from the device was updated
        /// </summary>
        public event Action InputChanged;
        /// <summary>
        /// The friendly display name of the controller
        /// </summary>
        public string DisplayName { get { return deviceInstance.ProductName; } }
        /// <summary>
        /// The GUID of the controller
        /// </summary>
        public Guid Id { get { return deviceInstance.ProductGuid; } }
        /// <summary>
        /// If the controller has DPad
        /// </summary>
        public bool HasDPad { get { return joystick.Capabilities.PovCount > 0; } }
        /// <summary>
        /// If the controller has axes
        /// </summary>
        public bool HasAxes { get { return joystick.Capabilities.AxesCount > 0; } }
        /// <summary>
        /// If the controller has buttons
        /// </summary>
        public int ButtonCount { get { return joystick.Capabilities.ButtonCount; } }
        
        private readonly DeviceInstance deviceInstance;
        private readonly Joystick joystick;
        
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
            joystick.Dispose();
        }

        /// <summary>
        /// Display name followed by the deviceID.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DisplayName + "(" + Id + ")";
        }
        
        /// <summary>
        /// Gets the current state of the DPad.
        /// </summary>
        /// <returns></returns>
        public DPadDirection GetDPad()
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

        /// <summary>
        /// Gets the current state of the inputTpye.
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        public double Get(DirectInputTypes inputType)
        {
            if (inputType.IsAxis())
            {
                return (GetAxisValue(inputType - DirectInputTypes.Axis1 + 1)) / (double)ushort.MaxValue;
            }
            if (inputType.IsButton())
            {
                return GetButtonValue(inputType - DirectInputTypes.Button1 + 1) ? 1d : 0d;
            }
            return 0;
        }

        /// <summary>
        /// Gets the current boolean value of the inputTpye.
        /// </summary>
        /// <param name="inputType">Type of input</param>
        /// <returns>Value</returns>
        public bool GetBool(DirectInputTypes inputType)
        {
            return Get(inputType) > 0.5;
        }

        /// <summary>
        /// Refreshes the current state.
        /// </summary>
        /// <returns></returns>
        public bool RefreshInput()
        {
            Result result = joystick.Poll();
            if (result.IsSuccess)
            {
                InputChanged?.Invoke();
            }
            return result.IsSuccess;
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
    }
}
