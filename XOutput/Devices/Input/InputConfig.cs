using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices.Input
{
    public class InputConfig
    {
        /// <summary>
        /// Enables the force feedback for the controller.
        /// </summary>
        public bool ForceFeedback { get; set; }

        public InputConfig()
        {
            ForceFeedback = false;
        }

        public InputConfig(int forceFeedbackCount)
        {
            ForceFeedback = forceFeedbackCount > 0;
        }
    }
}
