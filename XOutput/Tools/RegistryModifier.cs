using Microsoft.Win32;
using System;
using System.Diagnostics;
using XOutput.Logging;

namespace XOutput.Tools
{
    public sealed class RegistryModifier : IRegistryModifierService
    {
        /// <summary>
        /// Startup registry key.
        /// </summary>
        public static readonly string AutostartRegistry = Registry.CurrentUser.ToString() + @"\Software\Microsoft\Windows\CurrentVersion\Run";
        /// <summary>
        /// XOutput registry value
        /// </summary>
        public const string AutostartValueKey = "XOutput";
        /// <summary>
        /// Autostart command line parameters.
        /// </summary>
        public const string AutostartParams = " --minimized";

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(RegistryModifier));

        private readonly IRegistryModifierService registryModifierService;
        private readonly IRegistryModifierService externalRegistryModifierService;

        [ResolverMethod]
        public RegistryModifier(RegistryModifierService registryModifierService, ExternalRegistryModifierService externalRegistryModifierService)
        {
            this.registryModifierService = registryModifierService;
            this.externalRegistryModifierService = externalRegistryModifierService;
        }

        /// <summary>
        /// Activates autostart.
        /// </summary>
        public void SetAutostart()
        {
            var filename = Process.GetCurrentProcess().MainModule.FileName;
            string value = $"\"{filename}\" {AutostartParams}";
            SetValue(AutostartRegistry, AutostartValueKey, value);
        }

        /// <summary>
        /// Deactivates autostart.
        /// </summary>
        public void ClearAutostart()
        {
            DeleteValue(AutostartRegistry, AutostartValueKey);
        }

        public bool GetAutostart()
        {
            return (GetValue(AutostartRegistry, AutostartValueKey) as bool?) == true;
        }

        public bool KeyExists(string key)
        {
            return Try(r => r.KeyExists(key));
        }

        public void DeleteTree(string key)
        {
            Try(r => r.DeleteTree(key));
        }

        public void CreateKey(string key)
        {
            Try(r => r.CreateKey(key));
        }

        public object GetValue(string key, string value)
        {
            return Try(r => r.GetValue(key, value));
        }

        public void SetValue(string key, string value, object newValue)
        {
            Try(r => r.SetValue(key, value, newValue));
        }

        public void DeleteValue(string key, string value)
        {
            Try(r => r.DeleteValue(key, value));
        }

        private void Try(Action<IRegistryModifierService> calculate)
        {
            Try<object>(r =>
            {
                calculate(r);
                return null;
            });
        }

        private T Try<T>(Func<IRegistryModifierService, T> calculate)
        {
            try
            {
                return calculate(registryModifierService);
            }
            catch (Exception)
            {
                // Continue
            }
            try
            {
                return calculate(externalRegistryModifierService);
            }
            catch (Exception)
            {
                // Continue
            }
            return default;
        }
    }
}
