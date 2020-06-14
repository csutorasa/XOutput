using XOutput.Devices.Input;
using XOutput.Emulation.Ds4;

namespace XOutput.Mapping.Controller.Ds4
{
    public class Ds4Controller : ControllerBase<Ds4InputTypes>
    {
        private Ds4Device device;

        public void Start(IDs4Emulator emulator)
        {
            device = emulator.CreateDs4Device();
        }
        public void Stop()
        {
            device.Close();
        }

        protected override double GetDefaultValue(Ds4InputTypes input)
        {
            return input.GetDefaultValue();
        }

        protected override void InputChanged(DeviceInputChangedEventArgs args)
        {
            device.SendInput(new Ds4Input
            {
                Circle = GetBoolValue(Ds4InputTypes.Circle),
                Cross = GetBoolValue(Ds4InputTypes.Cross),
                Triangle = GetBoolValue(Ds4InputTypes.Triangle),
                Square = GetBoolValue(Ds4InputTypes.Square),
                L1 = GetBoolValue(Ds4InputTypes.L1),
                L3 = GetBoolValue(Ds4InputTypes.L3),
                R1 = GetBoolValue(Ds4InputTypes.R1),
                R3 = GetBoolValue(Ds4InputTypes.R3),
                Options = GetBoolValue(Ds4InputTypes.Options),
                Share = GetBoolValue(Ds4InputTypes.Share),
                Ps = GetBoolValue(Ds4InputTypes.Ps),
                Up = GetBoolValue(Ds4InputTypes.Up),
                Down = GetBoolValue(Ds4InputTypes.Down),
                Left = GetBoolValue(Ds4InputTypes.Left),
                Right = GetBoolValue(Ds4InputTypes.Right),
                LX = GetValue(Ds4InputTypes.LX),
                LY = GetValue(Ds4InputTypes.LY),
                RX = GetValue(Ds4InputTypes.RX),
                RY = GetValue(Ds4InputTypes.RY),
                L2 = GetValue(Ds4InputTypes.L2),
                R2 = GetValue(Ds4InputTypes.R2),
            });
        }
    }
}
