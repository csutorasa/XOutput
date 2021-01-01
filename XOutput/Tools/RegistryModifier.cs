using Microsoft.Win32;
using System.Diagnostics;
using XOutput.Logging;

namespace XOutput.Tools
{
    public sealed class RegistryModifier
    {
        /// <summary>
        /// Startup registry key.
        /// </summary>
        public const string AutostartRegistry = @"Software\Microsoft\Windows\CurrentVersion\Run";
        /// <summary>
        /// XOutput registry value
        /// </summary>
        public const string AutostartValueKey = "XOutput";
        /// <summary>
        /// Autostart command line parameters.
        /// </summary>
        public const string AutostartParams = " --minimized";

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(RegistryModifier));

        /// <summary>
        /// Gets or sets the autostart.
        /// </summary>
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
                {
                    SetAutostart();
                }
                else
                {
                    ClearAutostart();
                }
            }
        }

        /// <summary>
        /// Activates autostart.
        /// </summary>
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

        /// <summary>
        /// Deactivates autostart.
        /// </summary>
        public void ClearAutostart()
        {
            using (var key = GetRegistryKey())
            {
                key.DeleteValue(AutostartValueKey);
                logger.Debug($"{AutostartValueKey} registry is deleted");
            }
        }

        private static RegistryKey GetRegistryKey(bool writeable = true)
        {
            return Registry.CurrentUser.OpenSubKey(AutostartRegistry, writeable);
        }

        public static bool KeyExists(RegistryKey registryKey, string subkey)
        {
            using (var registry = registryKey.OpenSubKey(subkey))
            {
                return registry != null;
            }
        }

        public static void DeleteTree(RegistryKey registryKey, string subkey)
        {
            registryKey.DeleteSubKeyTree(subkey, false);
            registryKey.Close();
        }

        public static void CreateKey(RegistryKey registryKey, string subkey)
        {
            var registry = registryKey.CreateSubKey(subkey);
            registry.Close();
        }

        public static object GetValue(RegistryKey registryKey, string subkey, string key)
        {
            return registryKey.OpenSubKey(subkey).GetValue(key);
        }

        public static void SetValue(RegistryKey registryKey, string subkey, string key, object value)
        {
            using (var registry = registryKey.OpenSubKey(subkey, true))
            {
                registry.SetValue(key, value);
            }
        }

        public static string[] GetSubKeyNames(RegistryKey registryKey, string subkey)
        {
            return registryKey.OpenSubKey(subkey).GetSubKeyNames();
        }
    }
}
