using System.Collections.Generic;
using System.Linq;
using XOutput.DependencyInjection;

namespace XOutput.Mapping.Input
{
    public class MappableDevices
    {
        private readonly List<MappableDevice> devices = new List<MappableDevice>();
        private readonly object sync = new object();

        [ResolverMethod]
        public MappableDevices()
        {

        }

        public MappableDevice Create(string id, string name, List<MappableSource> sources)
        {
            var device = new MappableDevice(id, name, sources);
            lock (sync)
            {
                devices.Add(device);
            }
            return device;
        }

        public MappableDevice Find(string id)
        {
            return devices.FirstOrDefault(d => d.Id == id);
        }

        public bool Remove(MappableDevice device)
        {
            return devices.Remove(device);
        }
    }
}
