using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Core.Configuration
{
    public class JsonConfigurationManager : IConfigurationManager
    {
        private readonly FileManager fileManager;

        public JsonConfigurationManager(FileManager fileManager)
        {
            this.fileManager = fileManager;
        }

        public void Save<T>(string filePath, T configuration) where T : Configuration
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public T Load<T>(string filePath) where T : Configuration, new()
        {
            if (File.Exists(filePath))
            {
                var text = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(text);
            }
            return new T();
        }
    }
}
