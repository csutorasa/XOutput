using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Core.Configuration
{
    public abstract class ConfigurationManager
    {
        public void Save<T>(string filePath, T configuration) where T : Configuration
        {
            File.WriteAllText(filePath, ConfigurationToString(configuration));
        }

        public T Load<T>(string filePath, Func<T> defaultGetter) where T : Configuration
        {
            if (File.Exists(filePath))
            {
                var text = File.ReadAllText(filePath);
                return StringToConfiguration<T>(text);
            }
            if (defaultGetter != null)
            {
                return defaultGetter();
            }
            return default;
        }

        protected abstract string ConfigurationToString<T>(T configuration) where T : Configuration;
        protected abstract T StringToConfiguration<T>(string configuration) where T : Configuration;
    }
}
