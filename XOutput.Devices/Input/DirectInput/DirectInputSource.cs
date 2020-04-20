using SharpDX.DirectInput;
using System;

namespace XOutput.Devices.Input.DirectInput
{
    public class DirectInputSource : InputSource
    {
        private readonly Func<JoystickState, double> valueGetter;

        public DirectInputSource(IInputDevice inputDevice, string name, SourceTypes type, int offset, Func<JoystickState, double> valueGetter) : base(inputDevice, name, type, offset)
        {
            this.valueGetter = valueGetter;
        }

        public static DirectInputSource FromButton(IInputDevice device, DeviceObjectInstance instance, int index)
        {
            return new DirectInputSource(device, "Button " + instance.Usage, SourceTypes.Button, instance.Offset, state => state.Buttons[index] ? 1 : 0);
        }

        public static DirectInputSource[] FromDPad(IInputDevice device, int index)
        {
            return new DirectInputSource[] {
                new DirectInputSource(device, "DPad" + (index + 1) + " Up", SourceTypes.Dpad, 10000 + index * 4, state => GetDirection(state, index).HasFlag(DPadDirection.Up) ? 1 : 0),
                new DirectInputSource(device, "DPad" + (index + 1) + " Down", SourceTypes.Dpad, 10001 + index * 4, state => GetDirection(state,index).HasFlag(DPadDirection.Down) ? 1 : 0),
                new DirectInputSource(device, "DPad" + (index + 1) + " Left", SourceTypes.Dpad, 10002 + index * 4, state => GetDirection(state, index).HasFlag(DPadDirection.Left) ? 1 : 0),
                new DirectInputSource(device, "DPad" + (index + 1) + " Right", SourceTypes.Dpad, 10003 + index * 4, state => GetDirection(state, index).HasFlag(DPadDirection.Right) ? 1 : 0),
            };
        }

        private static DPadDirection GetDirection(JoystickState state, int index)
        {
            switch (state.PointOfViewControllers[index])
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
                    throw new ArgumentException(nameof(index));
            }
        }

        public static DirectInputSource FromAxis(IInputDevice device, DeviceObjectInstance instance)
        {
            SourceTypes type = SourceTypes.AxisX;
            if (instance.ObjectType == ObjectGuid.XAxis || instance.ObjectType == ObjectGuid.RxAxis)
            {
                type = SourceTypes.AxisX;
            }
            else if (instance.ObjectType == ObjectGuid.YAxis || instance.ObjectType == ObjectGuid.RyAxis)
            {
                type = SourceTypes.AxisY;
            }
            else if (instance.ObjectType == ObjectGuid.ZAxis || instance.ObjectType == ObjectGuid.RzAxis)
            {
                type = SourceTypes.AxisZ;
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
            return new DirectInputSource(device, name, type, instance.Offset, state => GetAxisValue(state, axisCount) / (double)ushort.MaxValue);
        }

        private static int GetAxisValue(JoystickState state, int instanceNumber)
        {
            if (instanceNumber < 0)
            {
                throw new ArgumentException(nameof(instanceNumber));
            }
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

        public static DirectInputSource FromSlider(IInputDevice device, DeviceObjectInstance instance, int index)
        {
            string name = instance.Name;
            return new DirectInputSource(device, name, SourceTypes.Slider, instance.Offset, state => GetSliderValue(state, index + 1) / (double)ushort.MaxValue);
        }

        private static int GetSliderValue(JoystickState state, int slider)
        {
            if (slider < 1)
            {
                throw new ArgumentException(nameof(slider));
            }
            return state.Sliders[slider - 1];
        }


        internal bool Refresh(JoystickState state)
        {
            double newValue = valueGetter(state);
            return RefreshValue(newValue);
        }
    }
}
