using System.Collections.Generic;
using System.Linq;
using XOutput.Common.Input;
using XOutput.DependencyInjection;

namespace XOutput.Mapping.Input
{
    public class InputDevices
    {
        private readonly List<InputDevice> devices = new List<InputDevice>();
        private readonly object sync = new object();

        [ResolverMethod]
        public InputDevices()
        {

        }

        public InputDevice Create(string id, string name, InputDeviceApi deviceApi, List<InputDeviceSourceWithValue> sources, List<InputDeviceTargetWithValue> targets)
        {
            var device = new InputDevice(id, name, deviceApi, sources, targets);
            lock (sync)
            {
                devices.Add(device);
            }
            return device;
        }

        public InputDevice Find(string id)
        {
            return devices.FirstOrDefault(d => d.Id == id);
        }

        public List<InputDevice> FindAll()
        {
            return devices.ToList();
        }

        public bool Remove(InputDevice device)
        {
            lock (sync)
            {
                return devices.Remove(device);
            }
        }
    }
}
