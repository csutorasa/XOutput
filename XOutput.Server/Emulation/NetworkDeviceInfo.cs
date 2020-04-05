using XOutput.Api.Devices;

namespace XOutput.Server.Emulation
{
    public class NetworkDeviceInfo
    {
        public IDevice Device { get; set; }

        public string IPAddress { get; set; }

        public DeviceTypes DeviceType { get; set; }

        public string Emulator { get; set; }
    }
}
