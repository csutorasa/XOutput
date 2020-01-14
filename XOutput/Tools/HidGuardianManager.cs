using Microsoft.Win32;
using System.Collections.Generic;
using XOutput.Core.DependencyInjection;

namespace XOutput.Tools
{
    public class HidGuardianManager
    {
        static readonly string PARAMETERS = Registry.LocalMachine.ToString() + "\\SYSTEM\\CurrentControlSet\\Services\\HidGuardian\\Parameters";
        static readonly string WHITE_LIST = PARAMETERS + "\\Whitelist";
        static readonly string AFFECTED_DEVICES = "AffectedDevices";

        private RegistryModifier registryModifier;

        [ResolverMethod]
        public HidGuardianManager(RegistryModifier registryModifier)
        {
            this.registryModifier = registryModifier;
        }

        public void ResetPid(int pid)
        {
            if (registryModifier.KeyExists(WHITE_LIST))
            {
                registryModifier.DeleteTree(WHITE_LIST);
            }
            registryModifier.CreateKey(WHITE_LIST);
            registryModifier.CreateKey(WHITE_LIST + "\\" + pid);
        }

        public List<string> GetDevices()
        {
            object value = registryModifier.GetValue(PARAMETERS, AFFECTED_DEVICES);
            if (value is string[])
            {
                return new List<string>(value as string[]);
            }
            return new List<string>();
        }

        public void AddAffectedDevice(string device)
        {
            if (device == null)
            {
                return;
            }
            var devices = GetDevices();
            devices.Add(device);
            registryModifier.SetValue(PARAMETERS, AFFECTED_DEVICES, devices.ToArray());
        }

        public bool RemoveAffectedDevice(string device)
        {
            if (device == null)
            {
                return false;
            }
            var devices = GetDevices();
            bool removed = devices.Remove(device);
            if (removed)
            {
                registryModifier.SetValue(PARAMETERS, AFFECTED_DEVICES, devices.ToArray());
            }
            return removed;
        }

        public bool IsAffected(string device)
        {
            var devices = GetDevices();
            return devices.Contains(device);
        }
    }
}
