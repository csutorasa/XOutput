using Newtonsoft.Json;
using System.IO;

namespace XOutput.Core.Configuration
{
    public class JsonConfigurationManager : ConfigurationManager
    {
        private readonly JsonSerializer jsonSerializer = new JsonSerializer();

        protected override void WriteConfiguration<T>(StreamWriter writer, T configuration)
        {
            jsonSerializer.Serialize(writer, configuration);
            writer.Flush();
        }

        protected override T ReadConfiguration<T>(StreamReader reader)
        {
            return (T) jsonSerializer.Deserialize(reader, typeof(T));
        }
    }
}
