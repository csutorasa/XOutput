using System;
using System.Collections.Generic;
using XOutput.Core.Configuration;

namespace XOutput.Devices.Input
{
    public interface IInputConfigManager
    {
        InputConfig LoadConfig(string id);
        void SaveConfig(string id, InputConfig config);
    }

    public abstract class InputConfigManager : IInputConfigManager
    {
        private readonly ConfigurationManager configurationManager;

        protected InputConfigManager(ConfigurationManager configurationManager)
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
