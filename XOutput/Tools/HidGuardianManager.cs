using Microsoft.Win32;
using System.Collections.Generic;

namespace XOutput.Tools
{
    public class HidGuardianManager
    {
        static readonly string PARAMETERS = "SYSTEM\\CurrentControlSet\\Services\\HidGuardian\\Parameters";
        static readonly string WHITE_LIST = PARAMETERS + "\\Whitelist";
        static readonly string AFFECTED_DEVICES = "AffectedDevices";

        public void ResetPid(int pid)
        {
            if (RegistryModifier.KeyExists(Registry.LocalMachine, WHITE_LIST))
            {
                RegistryModifier.DeleteTree(Registry.LocalMachine, WHITE_LIST);
            }
            RegistryModifier.CreateKey(Registry.LocalMachine, WHITE_LIST + "\\" + pid);
        }

        public List<string> GetDevices()
        {
            object value = RegistryModifier.GetValue(Registry.LocalMachine, PARAMETERS, AFFECTED_DEVICES);
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
            RegistryModifier.SetValue(Registry.LocalMachine, PARAMETERS, AFFECTED_DEVICES, devices.ToArray());
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
                RegistryModifier.SetValue(Registry.LocalMachine, PARAMETERS, AFFECTED_DEVICES, devices.ToArray());
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
