using NLog;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Threading;

namespace XOutput.Devices.Input
{
    public class InputDeviceManager
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly List<IInputDeviceProvider> inputDeviceProviders;
        private readonly ThreadContext readThreadContext;

        [ResolverMethod]
        public InputDeviceManager(List<IInputDeviceProvider> inputDeviceProviders)
        {
            this.inputDeviceProviders = inputDeviceProviders;
            readThreadContext = ThreadCreator.CreateLoop($"Input device manager refresh", RefreshLoop, 5000).Start();
        }

        private void RefreshLoop()
        {
            foreach(var provider in inputDeviceProviders)
            {
                provider.SearchDevices();
            }
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
