using System;
using System.Collections.Generic;
using XOutput.Core.Configuration;

namespace XOutput.Devices.Input
{
    public class IgnoredDevicesConfig : ConfigurationBase, IEquatable<IgnoredDevicesConfig>
    {
        public List<string> IgnoredHardwareIds { get; set; }

        public IgnoredDevicesConfig()
        {
            IgnoredHardwareIds = new List<string>();
        }

        public bool Equals(IgnoredDevicesConfig other)
        {
            return Equals(IgnoredHardwareIds, other.IgnoredHardwareIds);
        }
    }
}
