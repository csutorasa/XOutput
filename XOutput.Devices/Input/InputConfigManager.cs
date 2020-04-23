using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;

namespace XOutput.Devices.Input
{
    public class InputConfigManager
    {
        private readonly ConfigurationManager configurationManager;

        [ResolverMethod]
        public InputConfigManager(ConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
        }

        public InputConfig LoadConfig(string id)
        {
            return configurationManager.Load($"conf/input/{id}.json", () => new InputConfig());
        }

        public void SaveConfig(string id, InputConfig config)
        {
            configurationManager.Save($"conf/input/{id}.json", config);
        }
    }
}
