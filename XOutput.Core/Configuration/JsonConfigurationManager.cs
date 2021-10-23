using System.IO;
using System.Text.Json;

namespace XOutput.Configuration
{
    public class JsonConfigurationManager : ConfigurationManager
    {
        protected override void WriteConfiguration<T>(StreamWriter writer, T configuration)
        {
            string text = JsonSerializer.Serialize(configuration);
            writer.Write(text);
            writer.Flush();
        }

        protected override T ReadConfiguration<T>(StreamReader reader)
        {
            string text = reader.ReadToEnd();
            return JsonSerializer.Deserialize<T>(text);
        }

        protected override string GetFilePath(string path)
        {
            return path + ".json";
        }
    }
}
