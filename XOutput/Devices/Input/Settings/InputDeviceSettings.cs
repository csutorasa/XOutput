using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices.Input.Settings
{
    public class InputDeviceSettings
    {
        public Dictionary<InputType, InputSettings> InputSettings { get; set; }
        [JsonIgnore]
        public IInputDevice Device { get; set; }
    }
}
