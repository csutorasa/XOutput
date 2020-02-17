using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices.Mapper
{
    public abstract class Mapper
    {
        public string Device { get; set; }
        public int InputId { get; set; }
    }
}
