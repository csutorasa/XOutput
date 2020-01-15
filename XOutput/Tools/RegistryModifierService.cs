using Microsoft.Win32;
using System;
using System.Collections.Generic;
using XOutput.Core.DependencyInjection;

namespace XOutput.Tools
{
    public sealed class RegistryModifierService : IRegistryModifierService
    {
        private Dictionary<string, RegistryKey> mapping = new Dictionary<string, RegistryKey>();

        [ResolverMethod]
        public RegistryModifierService()
        {
            fillMapping(Registry.LocalMachine);
            fillMapping(Registry.CurrentUser);
            fillMapping(Registry.ClassesRoot);
            fillMapping(Registry.CurrentConfig);
        }

        private void fillMapping(RegistryKey key)
        {
            mapping.Add(key.ToString(), key);
        }

        private RegistryKey GetRootRegistryKey(string key)
        {
            string root = key.Substring(0, key.IndexOf('\\'));
            var result = mapping[root];
            if (result == null)
            {
                throw new ArgumentException(nameof(key));
            }
            return result;
        }

        public bool KeyExists(string key)
        {
            string subkey = key.Substring(key.IndexOf('\\') + 1);
            using (var registryKey = GetRootRegistryKey(key))
            {
                return registryKey.OpenSubKey(subkey) != null;
            }
        }

        public void DeleteTree(string key)
        {
            string subkey = key.Substring(key.IndexOf('\\') + 1);
            using (var registryKey = GetRootRegistryKey(key))
            {
                registryKey.DeleteSubKeyTree(subkey);
            }
        }

        public void CreateKey(string key)
        {
            string subkey = key.Substring(key.IndexOf('\\') + 1);
            using (var registryKey = GetRootRegistryKey(key))
            {
                registryKey.CreateSubKey(subkey);
            }
        }

        public object GetValue(string key, string value)
        {
            return Registry.GetValue(key, value, null);
        }

        public void SetValue(string key, string value, object newValue)
        {
            Registry.SetValue(key, value, newValue);
        }

        public void DeleteValue(string key, string value)
        {
            string subkey = key.Substring(key.IndexOf('\\') + 1);
            using (var registryKey = GetRootRegistryKey(key))
            {
                registryKey.OpenSubKey(subkey).DeleteValue(value);
            }
        }
    }
}
