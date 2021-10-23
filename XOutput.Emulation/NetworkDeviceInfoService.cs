using System.Collections.Generic;
using System.Linq;
using XOutput.DependencyInjection;

namespace XOutput.Emulation
{
    public class NetworkDeviceInfoService
    {
        private readonly List<NetworkDeviceInfo> connectedDevices = new List<NetworkDeviceInfo>();

        [ResolverMethod]
        public NetworkDeviceInfoService()
        {

        }

        public void Add(NetworkDeviceInfo deviceInfo)
        {
            connectedDevices.Add(deviceInfo);
        }

        public void StopAndRemove(string id)
        {
            var device = connectedDevices.Where(di => di.Device.Id == id).First();
            device.Device.Close();
            connectedDevices.Remove(device);
        }

        public IEnumerable<NetworkDeviceInfo> GetConnectedDevices() => connectedDevices.ToList();
    }
}
