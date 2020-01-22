using XOutput.Api.Devices;

namespace XOutput.Server.Emulation
{
    public interface IEmulator
    {
        bool Installed { get; }
        string Name { get; }
        DeviceTypes[] SupportedDeviceTypes { get; }
        void Close();
    }
}
