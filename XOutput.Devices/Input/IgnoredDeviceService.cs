using System.Collections.Generic;
using XOutput.Core.DependencyInjection;

namespace XOutput.Devices.Input
{
    public class IgnoredDeviceService
    {
        private const string EmulatedSCPID = "028e045e-0000-0000-0000-504944564944";

        private readonly List<string> ignoredInstances = new List<string>();
        private readonly List<string> ignoredProducts = new List<string>();

        [ResolverMethod]
        public IgnoredDeviceService()
        {
            AddIgnoredProduct(EmulatedSCPID);
        }

        public void AddIgnoredInstance(string deviceId)
        {
            ignoredInstances.Add(deviceId);
        }

        public void RemoveIgnoredInstance(string deviceId)
        {
            ignoredInstances.Remove(deviceId);
        }

        public void AddIgnoredProduct(string deviceId)
        {
            ignoredProducts.Add(deviceId);
        }

        public void RemoveIgnoredProduct(string deviceId)
        {
            ignoredProducts.Remove(deviceId);
        }

        public bool IsIgnored(string productId, string instnaceId)
        {
            return ignoredProducts.Contains(productId) && ignoredInstances.Contains(instnaceId);
        }
    }
}
