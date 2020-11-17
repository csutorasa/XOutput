using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Core.Configuration;

namespace XOutput.App.Devices.Input
{
    public class InputConfig : ConfigurationBase, IEquatable<InputConfig>
    {
        public bool Autostart { get; set; }

        public List<InputSourceConfig> Sources { get; set; }

        public InputConfig()
        {
            Autostart = false;
            Sources = new List<InputSourceConfig>();
        }

        public InputConfig(IInputDevice device)
        {
            Autostart = device.Running;
            Sources = device.Sources.Select(s => new InputSourceConfig
            {
                Offset = s.Offset,
                Deadzone = 0,
            }).ToList();
        }

        public bool Equals(InputConfig other)
        {
            return Equals(Autostart, other.Autostart) &&
                Equals(Sources, other.Sources);
        }
    }

    public class InputSourceConfig
    {
        public int Offset { get; set; }
        public double Deadzone { get; set; }

        public InputSourceConfig() {
            Deadzone = 0;
        }
    }
}
