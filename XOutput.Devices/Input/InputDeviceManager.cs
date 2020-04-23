using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Threading;

namespace XOutput.Devices.Input
{
    public class InputDeviceManager : IDisposable
    {
        private readonly List<IInputDeviceProvider> inputDeviceProviders;
        private readonly ThreadContext readThreadContext;
        private bool disposed;

        [ResolverMethod]
        public InputDeviceManager(List<IInputDeviceProvider> inputDeviceProviders)
        {
            this.inputDeviceProviders = inputDeviceProviders;
            readThreadContext = ThreadCreator.CreateLoop($"Input device manager refresh", RefreshLoop, 5000).Start();
        }

        private void RefreshLoop()
        {
            foreach (var provider in inputDeviceProviders)
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

        public bool SaveInputConfiguration(string id, InputConfig inputConfig)
        {
            return inputDeviceProviders.Any(p => p.SaveInputConfig(id, inputConfig));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                readThreadContext?.Cancel()?.Wait();
            }
            disposed = true;
        }
    }
}
