using XOutput.Api.Devices;

namespace XOutput.Server.Emulation
{
    public interface IDevice
    {
        DeviceTypes DeviceType { get; }
        void Close();
    }
}
