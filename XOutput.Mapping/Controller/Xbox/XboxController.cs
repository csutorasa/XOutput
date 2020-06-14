using XOutput.Devices.Input;
using XOutput.Emulation.Xbox;

namespace XOutput.Mapping.Controller.Xbox
{
    public class XboxController : ControllerBase<XboxInputTypes>
    {

        private XboxDevice device;

        public void Start(IXboxEmulator emulator)
        {
            device = emulator.CreateXboxDevice();
        }
        public void Stop()
        {
            device.Close();
        }
        protected override double GetDefaultValue(XboxInputTypes input)
        {
            return input.GetDefaultValue();
        }

        protected override void InputChanged(DeviceInputChangedEventArgs args)
        {
            device.SendInput(new XboxInput
            {
                A = GetBoolValue(XboxInputTypes.A),
                B = GetBoolValue(XboxInputTypes.B),
                X = GetBoolValue(XboxInputTypes.X),
                Y = GetBoolValue(XboxInputTypes.Y),
                L1 = GetBoolValue(XboxInputTypes.L1),
                L3 = GetBoolValue(XboxInputTypes.L3),
                R1 = GetBoolValue(XboxInputTypes.R1),
                R3 = GetBoolValue(XboxInputTypes.R3),
                Start = GetBoolValue(XboxInputTypes.Start),
                Back = GetBoolValue(XboxInputTypes.Back),
                Home = GetBoolValue(XboxInputTypes.Home),
                Up = GetBoolValue(XboxInputTypes.Up),
                Down = GetBoolValue(XboxInputTypes.Down),
                Left = GetBoolValue(XboxInputTypes.Left),
                Right = GetBoolValue(XboxInputTypes.Right),
                LX = GetValue(XboxInputTypes.LX),
                LY = GetValue(XboxInputTypes.LY),
                RX = GetValue(XboxInputTypes.RX),
                RY = GetValue(XboxInputTypes.RY),
                L2 = GetValue(XboxInputTypes.L2),
                R2 = GetValue(XboxInputTypes.R2),
            });
        }
    }
}

