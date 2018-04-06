using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Tools
{
    public sealed class RegistryModifier
    {
        public const string AutostartRegistry = @"Software\Microsoft\Windows\CurrentVersion\Run";
        public const string AutostartValueKey = "XOutput";
        public const string AutostartParams = " --minimized";

        private static RegistryModifier instance = new RegistryModifier();
        public static RegistryModifier Instance => instance;

        public bool Autostart
        {
            get
            {
                using (var key = GetRegistryKey())
                {
                    return key.GetValue(AutostartValueKey) != null;
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
                var filename = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                key.SetValue(AutostartValueKey, $"\"{filename}\" {AutostartParams}");
            }
        }

        public void ClearAutostart()
        {
            using (var key = GetRegistryKey())
            {
                key.DeleteValue(AutostartValueKey);
            }
        }

        private RegistryKey GetRegistryKey(bool writeable = true)
        {
            return Registry.CurrentUser.OpenSubKey(AutostartRegistry, writeable);
        }
    }
}
