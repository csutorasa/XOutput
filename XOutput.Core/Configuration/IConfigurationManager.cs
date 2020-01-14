using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Core.Configuration
{
    public interface IConfigurationManager
    {
        void Save<T>(string filePath, T configuration) where T : Configuration;

        T Load<T>(string filePath) where T : Configuration, new();
    }
}
