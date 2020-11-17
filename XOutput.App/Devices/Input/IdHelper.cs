using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace XOutput.App.Devices.Input
{
    public static class IdHelper
    {

        private static readonly Regex idRegex = new Regex("[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}");
        private static readonly Regex hidRegex = new Regex("(hid)#([^#]+)#([^#]+)");

        private static readonly SHA256 sha = SHA256.Create();
        private static Encoding encoding = Encoding.UTF8;

        public static string GetHardwareId(string path)
        {
            if (string.IsNullOrEmpty(path)) {
                return null;
            }
            var match = hidRegex.Match(path);
            if (match.Success)
            {
                return string.Join('\\', new string[] { match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value }).ToUpper();
            }
            if (path.Contains("hid#"))
            {
                path = path.Substring(path.IndexOf("hid#"));
                path = path.Replace('#', '\\');
                int first = path.IndexOf('\\');
                int second = path.IndexOf('\\', first + 1);
                if (second > 0)
                {
                    return path.Remove(second).ToUpper();
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
