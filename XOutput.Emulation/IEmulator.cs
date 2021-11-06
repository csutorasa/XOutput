using System.Collections.Generic;
using XOutput.Common;

namespace XOutput.Emulation
{
    public interface IEmulator
    {
        bool Installed { get; }
        Emulators Emulator { get; }
        IEnumerable<DeviceTypes> SupportedDeviceTypes { get; }
        void Close();
    }
}
