using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Logging;

namespace XOutput.Tools
{
    public sealed class RegistryModifier
    {
        public const string AutostartRegistry = @"Software\Microsoft\Windows\CurrentVersion\Run";
        public const string AutostartValueKey = "XOutput";
        public const string AutostartParams = " --minimized";

        private static RegistryModifier instance = new RegistryModifier();
        public static RegistryModifier Instance => instance;
        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(RegistryModifier));

        public bool Autostart
        {
            get
            {
                using (var key = GetRegistryKey())
                {
                    bool exists = key.GetValue(AutostartValueKey) != null;
                    logger.Debug($"{AutostartValueKey} registry is " + (exists ? "" : "not ") + "found");
                    return exists;
                }
            }
            set
            {
                if (value)
                    SetAutostart();
                else
                    ClearAutostart();
            }
        }

        public void SetAutostart()
        {
            using (var key = GetRegistryKey())
            {
                var filename = Process.GetCurrentProcess().MainModule.FileName;
                string value = $"\"{filename}\" {AutostartParams}";
                key.SetValue(AutostartValueKey, value);
                logger.Debug($"{AutostartValueKey} registry set to {value}");
            }
        }

        public void ClearAutostart()
        {
            using (var key = GetRegistryKey())
            {
                key.DeleteValue(AutostartValueKey);
                logger.Debug($"{AutostartValueKey} registry is deleted");
            }
        }

        private RegistryKey GetRegistryKey(bool writeable = true)
        {
            return Registry.CurrentUser.OpenSubKey(AutostartRegistry, writeable);
        }
    }
}
