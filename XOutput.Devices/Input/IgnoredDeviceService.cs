using System.Collections.Generic;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;

namespace XOutput.Devices.Input
{
    public class IgnoredDeviceService
    {
        private const string ConfigurationFilepath = "conf/input/ignore";
        private readonly ConfigurationManager configurationManager;
        private readonly IgnoredDevicesConfig config;

        [ResolverMethod]
        public IgnoredDeviceService(ConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
            config = configurationManager.Load(ConfigurationFilepath, () => new IgnoredDevicesConfig());
        }

        public void AddIgnoredHardwareId(string hardwareId)
        {
            config.IgnoredHardwareIds.Add(hardwareId);
            configurationManager.Save(config);
        }

        public void RemoveIgnoredHardwareId(string hardwareId)
        {
            config.IgnoredHardwareIds.Remove(hardwareId);
            configurationManager.Save(config);
        }

        public bool IsIgnored(string hardwareId)
        {
            return hardwareId != null && config.IgnoredHardwareIds.Contains(hardwareId);
        }
    }
}
