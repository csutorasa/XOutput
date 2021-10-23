using System.Collections.Generic;
using XOutput.Common;

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
