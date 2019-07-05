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
        /// Starts the mapping when connected.
        /// </summary>
        public bool StartWhenConnected { get; set; }

        /// <summary>
        /// Enables the force feedback for the controller.
        /// </summary>
        public bool ForceFeedback { get; set; }

        public InputConfig()
        {
            StartWhenConnected = false;
            ForceFeedback = false;
        }

        public InputConfig(int forceFeedbackCount)
        {
            StartWhenConnected = false;
            ForceFeedback = forceFeedbackCount > 0;
        }
    }
}
