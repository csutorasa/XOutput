using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using XOutput.Configuration;
using XOutput.DependencyInjection;

namespace XOutput.App.Devices.Input
{
    public sealed class IdHelper
    {

        private static readonly Regex idRegex = new Regex("[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}");
        private static readonly Regex hidRegex = new Regex("(hid)#([^#]+)#[^#]+");
        private static readonly Regex hidForRegistryRegex = new Regex("hid#(vid_[0-9a-f]{4}&pid_[0-9a-f]{4})[^#]*#([0-9a-f&]+)");

        private static readonly SHA256 sha = SHA256.Create();
        private static Encoding encoding = Encoding.UTF8;

        private readonly RegistryModifierService registryModifierService;

        [ResolverMethod]
        public IdHelper(RegistryModifierService registryModifierService)
        {
            this.registryModifierService = registryModifierService;
        }

        public string GetHardwareId(string path)
        {
            if (string.IsNullOrEmpty(path)) {
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
            return string.Join('\\', new string[] { match.Groups[1].Value, match.Groups[2].Value }).ToUpper();
        }

        private string GetHardwareIdFromRegistryWithHidMatch(Match match)
        {
            string path = $"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Enum\\USB\\{match.Groups[1].Value}";
            if (registryModifierService.KeyExists(path))
            {
                foreach (string subkey in registryModifierService.GetSubKeyNames(path))
                {
                    string parentIdPrefix = registryModifierService.GetValue<string>($"{path}\\{subkey}", "ParentIdPrefix");
                    if (parentIdPrefix == null || !match.Groups[2].Value.StartsWith(parentIdPrefix))
                    {
                        continue;
                    }
                    object registryHardwareIds = registryModifierService.GetValue($"{path}\\{subkey}", "HardwareID");
                    if (registryHardwareIds is string[])
                    {
                        return (registryHardwareIds as string[]).Select(id => id.Replace("USB\\", "HID\\")).FirstOrDefault();
                    }
                }
            }
            return null;
        }

        public static string GetUniqueId(string path)
        {
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            byte[] hash = sha.ComputeHash(encoding.GetBytes(path));
            
            foreach (byte b in hash)
            {                
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
