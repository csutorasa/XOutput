using Microsoft.Win32;
using System.Diagnostics;
using XOutput.Logging;

namespace XOutput.Tools
{
    public interface IRegistryModifierService
    {
        bool KeyExists(string key);
        void DeleteTree(string key);
        void CreateKey(string key);
        object GetValue(string key, string value);
        void SetValue(string key, string value, object newValue);
        void DeleteValue(string key, string value);
    }
}
