using System.Collections.Generic;
using XOutput.Configuration;
using XOutput.DependencyInjection;

namespace XOutput.App.Devices.Input
{
    public class IgnoredDeviceService
    {
        private const string ConfigurationFilepath = "conf/ignored devices";
        private readonly ConfigurationManager configurationManager;
        private readonly IgnoredDevicesConfig config;
        private readonly List<string> temporaryIgnore = new List<string>();

        [ResolverMethod]
        public IgnoredDeviceService(ConfigurationManager configurationManager)
        {
            this.configurationManager = configurationManager;
            config = configurationManager.Load(ConfigurationFilepath, () => new IgnoredDevicesConfig());
        }

        public void AddIgnore(string interfacePath)
        {
            config.IgnoredHardwareIds.Add(interfacePath);
            configurationManager.Save(config);
        }

        public void RemoveIgnore(string interfacePath)
        {
            config.IgnoredHardwareIds.Remove(interfacePath);
            configurationManager.Save(config);
        }


        public void AddTemporaryIgnore(string interfacePath)
        {
            temporaryIgnore.Add(interfacePath);
        }

        public void RemoveTemporaryIgnore(string interfacePath)
        {
            temporaryIgnore.Remove(interfacePath);
        }

        public bool IsIgnored(string interfacePath)
        {
            return interfacePath != null && (config.IgnoredHardwareIds.Contains(interfacePath) || temporaryIgnore.Contains(interfacePath));
        }
    }
}
