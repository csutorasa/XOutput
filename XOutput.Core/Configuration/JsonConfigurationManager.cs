using Newtonsoft.Json;

namespace XOutput.Core.Configuration
{
    public class JsonConfigurationManager : ConfigurationManager
    {
        protected override string ConfigurationToString<T>(T configuration)
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        protected override T StringToConfiguration<T>(string configuration)
        {
            return JsonConvert.DeserializeObject<T>(configuration);
        }
    }
}
