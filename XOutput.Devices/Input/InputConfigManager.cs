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

        public InputConfig LoadConfig(IInputDevice device)
        {
            return configurationManager.Load($"conf/{device.UniqueId}/{device.InputMethod}", () => new InputConfig());
        }

        public void SaveConfig(IInputDevice device)
        {
            configurationManager.Save($"conf/{device.UniqueId}/{device.InputMethod}", device.InputConfiguration);
        }
    }
}
