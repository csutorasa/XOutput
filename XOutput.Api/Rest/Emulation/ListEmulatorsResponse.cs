using System.Collections.Generic;

namespace XOutput.Rest.Emulation
{
    public class EmulatorResponse
    {
        public bool Installed { get; set; }
        public List<string> SupportedDeviceTypes { get; set; }
    }
}
