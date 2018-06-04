using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.Input;

namespace XOutput.Devices.XInput.Settings
{
    public class ForceFeedbackSettings
    {
        public string DeviceName { get; set; }
        [JsonIgnore]
        public IInputDevice Device { get; set; }
    }
}
