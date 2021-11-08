using System.Collections.Generic;
using System.Linq;
using XOutput.DependencyInjection;

namespace XOutput.Emulation
{
    public class EmulatedControllersService
    {
        private readonly List<NetworkDeviceInfo> connectedDevices = new List<NetworkDeviceInfo>();

        [ResolverMethod]
        public EmulatedControllersService()
        {

        }

        public void Add(NetworkDeviceInfo deviceInfo)
        {
            connectedDevices.Add(deviceInfo);
        }

        public bool StopAndRemove(string id)
        {
            var device = connectedDevices.Where(di => di.Device.Id == id).FirstOrDefault();
            if (device == null) {
                return false;
            }
            device.Device.Close();
            connectedDevices.Remove(device);
            return true;
        }

        public IEnumerable<NetworkDeviceInfo> GetConnectedDevices() => connectedDevices.ToList();
    }
}
