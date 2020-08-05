using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Threading;

namespace XOutput.Devices.Input
{
    public class InputDeviceManager : IDisposable
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly InputConfigManager inputConfigManager;
        private readonly List<IInputDeviceProvider> inputDeviceProviders;
        private readonly List<InputDeviceHolder> devices = new List<InputDeviceHolder>();
        private readonly object lockObject = new object();
        private readonly ThreadContext readThreadContext;
        private bool disposed;

        [ResolverMethod]
        public InputDeviceManager(InputConfigManager inputConfigManager, List<IInputDeviceProvider> inputDeviceProviders)
        {
            this.inputConfigManager = inputConfigManager;
            this.inputDeviceProviders = inputDeviceProviders;
            foreach (var inputDeviceProvider in inputDeviceProviders)
            {
                inputDeviceProvider.Connected += Connected;
                inputDeviceProvider.Disconnected += Disconnected;
            }
            readThreadContext = ThreadCreator.CreateLoop($"Input device manager refresh", RefreshLoop, 5000).Start();
        }

        private void Connected(object sender, DeviceConnectedEventArgs e)
        {
            lock (lockObject) {
                var device = devices.FirstOrDefault(d => d.UniqueId == e.Device.UniqueId);
                if (device == null)
                {
                    device = new InputDeviceHolder(e.Device.DisplayName, e.Device.InterfacePath, e.Device.UniqueId, e.Device.HardwareID);
                    devices.Add(device);
                }
                device.SetInput(e.Device.InputMethod, e.Device);
            }
        }

        private void Disconnected(object sender, DeviceDisconnectedEventArgs e)
        {
            lock (lockObject) {
                var device = devices.FirstOrDefault(d => d.UniqueId == e.Device.UniqueId);
                if (device?.RemoveInput(e.Device.InputMethod) ?? false) {
                    devices.Remove(device);
                }
            }
        }

        private void RefreshLoop()
        {
            foreach (var provider in inputDeviceProviders)
            {
                try
                {
                    provider.SearchDevices();
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Failed to search for devices from provider {provider.GetType().Name}");
                }
            }
        }


        public List<InputDeviceHolder> GetInputDevices()
        {
            return devices.ToList();
        }

        public InputDeviceHolder FindInputDevice(string id)
        {
            return devices.FirstOrDefault(d => d.UniqueId == id);
        }

        public bool ChangeInputConfiguration(string id, InputDeviceMethod method, Action<InputConfig> configuration)
        {
            var inputDevice = FindInputDevice(id)?.FindInput(method);
            if (inputDevice == null)
            {
                return false;
            }
            configuration(inputDevice.InputConfiguration);
            inputConfigManager.SaveConfig(inputDevice);
            return true;
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
                foreach (var inputDeviceProvider in inputDeviceProviders)
                {
                    inputDeviceProvider.Connected -= Connected;
                    inputDeviceProvider.Disconnected -= Disconnected;
                }
                readThreadContext?.Cancel()?.Wait();
            }
            disposed = true;
        }
    }
}
