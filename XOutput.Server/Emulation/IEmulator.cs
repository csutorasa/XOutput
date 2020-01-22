using System.Collections.Generic;
using XOutput.Api.Devices;

namespace XOutput.Server.Emulation
{
    public interface IEmulator
    {
        bool Installed { get; }
        string Name { get; }
        IEnumerable<DeviceTypes> SupportedDeviceTypes { get; }
        void Close();
    }
}
