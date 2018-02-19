using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.UI.Resources
{
    public static class ErrorMessage
    {
        public static string Warning { get { return "Warning"; } }
        public static string Error { get { return "Error"; } }
        public static string SaveSettingsError { get { return "Failed to save {0}!" + Environment.NewLine + "{1}"; } }
        public static string LoadSettingsError { get { return "Failed to load {0}!" + Environment.NewLine + "{1}"; } }
        public static string SCPNotInstalledError { get { return "Please install VIGEm!" + Environment.NewLine + "{0}"; } }
    }
}
