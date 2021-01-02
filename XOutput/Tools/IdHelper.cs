using Microsoft.Win32;
using System.Linq;
using System.Text.RegularExpressions;

namespace XOutput.Tools
{
    public static class IdHelper
    {

        private static readonly Regex idRegex = new Regex("[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}");
        private static readonly Regex hidRegex = new Regex("(hid)#([^#]+)#[^#]+");
        private static readonly Regex hidForRegistryRegex = new Regex("hid#(vid_[0-9a-f]{4}&pid_[0-9a-f]{4})[^#]*#([0-9a-f&]+)");

        public static string GetHardwareId(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            var match = hidForRegistryRegex.Match(path);
            if (match.Success)
            {
                string harwareIdFromRegistry = GetHardwareIdFromRegistryWithHidMatch(match);
                if (harwareIdFromRegistry != null)
                {
                    return harwareIdFromRegistry;
                }
            }
            match = hidRegex.Match(path);
            if (match.Success)
            {
                return GetHardwareIdFromHidMatch(match);
            }
            if (path.Contains("hid#"))
            {
                return GetHardwareIdFromInstancePath(path);
            }
            return null;
        }

        private static string GetHardwareIdFromInstancePath(string path)
        {
            path = path.Substring(path.IndexOf("hid#"));
            path = path.Replace('#', '\\');
            int first = path.IndexOf('\\');
            int second = path.IndexOf('\\', first + 1);
            if (second > 0)
            {
                return path.Remove(second).ToUpper();
            }
            return path;
        }

        private static string GetHardwareIdFromHidMatch(Match match)
        {
            return string.Join("\\", new string[] { match.Groups[1].Value, match.Groups[2].Value }).ToUpper();
        }

        private static string GetHardwareIdFromRegistryWithHidMatch(Match match)
        {
            string path = $"SYSTEM\\CurrentControlSet\\Enum\\USB\\{match.Groups[1].Value}";
            if (RegistryModifier.KeyExists(Registry.LocalMachine, path))
            {
                foreach (string subkey in RegistryModifier.GetSubKeyNames(Registry.LocalMachine, path))
                {
                    string parentIdPrefix = RegistryModifier.GetValue(Registry.LocalMachine, $"{path}\\{subkey}", "ParentIdPrefix") as string;
                    if (parentIdPrefix == null || !match.Groups[2].Value.StartsWith(parentIdPrefix))
                    {
                        continue;
                    }
                    object registryHardwareIds = RegistryModifier.GetValue(Registry.LocalMachine, $"{path}\\{subkey}", "HardwareID");
                    if (registryHardwareIds is string[])
                    {
                        return (registryHardwareIds as string[]).Select(id => id.Replace("USB\\", "HID\\")).FirstOrDefault();
                    }
                }
            }
            return null;
        }
    }
}
