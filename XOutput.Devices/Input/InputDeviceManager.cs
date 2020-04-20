using NLog;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Core.DependencyInjection;

namespace XOutput.Devices.Input
{
    public class InputDeviceManager
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly List<IInputDeviceProvider> inputDeviceProviders;

        [ResolverMethod]
        public InputDeviceManager(List<IInputDeviceProvider> inputDeviceProviders)
        {
            this.inputDeviceProviders = inputDeviceProviders;
        }

        public List<IInputDevice> GetInputDevices()
        {
            return inputDeviceProviders.SelectMany(p => p.GetActiveDevices()).ToList();
        }

        public IInputDevice FindInputDevice(string id)
        {
            return inputDeviceProviders.SelectMany(p => p.GetActiveDevices()).FirstOrDefault(d => d.UniqueId == id);
        }
    }
}
