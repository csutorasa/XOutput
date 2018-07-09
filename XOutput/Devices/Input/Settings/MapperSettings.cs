using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.Input;

namespace XOutput.Devices.Input.Settings
{
    public class InputSettings
    {
        public double Deadzone { get; set; }
        public double AntiDeadzone { get; set; }
    }
}
