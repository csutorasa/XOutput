using XOutput.Core.DependencyInjection;
using XOutput.Devices.Input;
using XOutput.Devices.Input.DirectInput;

namespace XOutput.Devices
{
    public static class DevicesConfiguration
    {
        [ResolverMethod]
        public static IInputDeviceProvider GetDirectInputDeviceProvider()
        {
            return new DirectInputDeviceProvider();
        }
    }
}
