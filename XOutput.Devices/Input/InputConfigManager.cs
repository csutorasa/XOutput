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
            return configurationManager.Load($"conf/input/{id}", () => new InputConfig());
        }

        public void SaveConfig(string id, InputConfig config)
        {
            configurationManager.Save(config);
        }
    }
}
