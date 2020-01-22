using System.Collections.Generic;
using XOutput.Api.Devices;

namespace XOutput.Api.Emulation
{
    public class ListEmulatorsResponse
    {
        public Dictionary<string, EmulatorResponse> Emulators { get; set; }
    }

    public class EmulatorResponse
    {
        public bool Installed { get; set; }
        public List<string> SupportedDeviceTypes { get; set; }
    }
}
