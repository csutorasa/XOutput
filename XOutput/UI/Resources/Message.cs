using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.UI.Resources
{
    public static class Message
    {
        public static string About { get { return "About"; } }
        public static string AboutContent { get { return "Created by: https://github.com/csutorasa"; } }
        public static string Information { get { return "Information"; } }
        public static string SaveSettings { get { return "Successfully saved settings to {0}."; } }
        public static string LoadSettings { get { return "Successfully loaded settings from {0}."; } }
        public static string ControllerConnected { get { return "{0} is connected."; } }
        public static string ControllerDisconnected { get { return "{0} is disconnected."; } }
        public static string EmulationStarted { get { return "{0} is emulated as controller #{1}."; } }
        public static string EmulationStopped { get { return "{0} is no longer emulated."; } }
    }
}
