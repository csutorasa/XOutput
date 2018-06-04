using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.XInput;

namespace XOutput.Devices.XInput.Settings
{
    public class OutputDeviceSettings
    {
        public bool StartWhenConnected { get; set; }
        public List<ForceFeedbackSettings> ForceFeedbackDevices { get; set; }
        public DPadSettings DPadSettings { get; set; }
        public Dictionary<XInputTypes, MapperSettings> Mapping { get; set; }
    }
}
