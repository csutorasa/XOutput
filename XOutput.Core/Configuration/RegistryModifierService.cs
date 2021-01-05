using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace XOutput.Core.Configuration
{
    public sealed class RegistryModifierService
    {
        private readonly Dictionary<string, RegistryKey> mapping = new Dictionary<string, RegistryKey>();

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

        public bool DeleteTree(string key)
        {
            if (!KeyExists(key))
            {
                return false;
            }
            string subkey = key.Substring(key.IndexOf('\\') + 1);
            using (var registryKey = GetRootRegistryKey(key))
            {
                registryKey.DeleteSubKeyTree(subkey);
            }
            return true;
        }

        public bool CreateKey(string key)
        {
            if (KeyExists(key))
            {
                return false;
            }
            string subkey = key.Substring(key.IndexOf('\\') + 1);
            using (var registryKey = GetRootRegistryKey(key))
            {
                registryKey.CreateSubKey(subkey);
            }
            return true;
        }

        public object GetValue(string key, string value, bool createKeyIfNotExists = true)
        {
            if (createKeyIfNotExists)
            {
                CreateKey(key);
            }
            return Registry.GetValue(key, value, null);
        }

        public T GetValue<T>(string key, string value, bool createKeyIfNotExists = true)
        {
            if (createKeyIfNotExists)
            {
                CreateKey(key);
            }
            return (T) Registry.GetValue(key, value, null);
        }

        public void SetValue(string key, string value, object newValue, bool createKeyIfNotExists = true)
        {
            if (createKeyIfNotExists)
            {
                CreateKey(key);
            }
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

        public string[] GetSubKeyNames(string key, bool createKeyIfNotExists = true)
        {
            if (createKeyIfNotExists)
            {
                CreateKey(key);
            }
            string subkey = key.Substring(key.IndexOf('\\') + 1);
            using (var registryKey = GetRootRegistryKey(key))
            {
                return registryKey.OpenSubKey(subkey).GetSubKeyNames();
            }
        }
    }
}
