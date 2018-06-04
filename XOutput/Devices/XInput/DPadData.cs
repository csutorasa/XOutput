using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.Input;

namespace XOutput.Devices.XInput
{
    /// <summary>
    /// Contains mapping data to Xinput conversion.
    /// </summary>
    public class DPadData
    {
        /// <summary>
        /// From data type
        /// </summary>
        public IInputDevice InputDevice { get; set; }
        /// <summary>
        /// Selected DPad index
        /// </summary>
        public int Selected { get; set; }
        /// <summary>
        /// If the input device has DPad.
        /// </summary>
        public bool HasDPad => InputDevice != null && InputDevice.DPads.Count() > 0;

        public DPadData()
        {
            InputDevice = null;
            Selected = -1;
        }

        public DPadDirection GetDirection()
        {
            if (InputDevice == null || Selected < 0 || InputDevice.DPads.Count() <= Selected)
            {
                return DPadDirection.None;
            }
            return InputDevice.DPads.ElementAt(Selected);
        }
    }
}
