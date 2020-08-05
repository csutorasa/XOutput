using System;
using System.Collections.Generic;
using XOutput.Core.Configuration;

namespace XOutput.Devices.Input
{
    public class InputConfig : ConfigurationBase, IEquatable<InputConfig>
    {
        public bool Autostart { get; set; }

        public InputConfig()
        {
            Autostart = false;
        }

        public bool Equals(InputConfig other)
        {
            return Equals(Autostart, other.Autostart);
        }
    }
}
