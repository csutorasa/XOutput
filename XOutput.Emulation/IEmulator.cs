using System.Collections.Generic;

namespace XOutput.Emulation
{
    public interface IEmulator
    {
        bool Installed { get; }
        string Name { get; }
        IEnumerable<DeviceTypes> SupportedDeviceTypes { get; }
        void Close();
    }
}
