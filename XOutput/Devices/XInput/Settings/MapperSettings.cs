using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices.XInput.Settings
{
    public class MapperSettings
    {
        public string Device { get; set; }
        public string Type { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }
}
