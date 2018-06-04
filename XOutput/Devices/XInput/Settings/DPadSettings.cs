using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.Input;

namespace XOutput.Devices.XInput.Settings
{
    public class DPadSettings
    {
        public string DeviceName { get; set; }
        public int Selected { get; set; }
        [JsonIgnore]
        public IInputDevice Device { get; set; }
        /// <summary>
        /// If the input device has DPad.
        /// </summary>
        [JsonIgnore]
        public bool HasDPad => Device != null && Device.DPads.Count() > 0;


        public DPadDirection GetDirection()
        {
            if (Device == null || Selected < 0 || Device.DPads.Count() <= Selected)
            {
                return DPadDirection.None;
            }
            return Device.DPads.ElementAt(Selected);
        }

    }
}
