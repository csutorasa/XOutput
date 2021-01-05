using NLog;
using System;
using System.Collections.Generic;
using XOutput.Core.DependencyInjection;

namespace XOutput.App.Devices.Input.Mouse
{
    public sealed class MouseDeviceProvider : IInputDeviceProvider
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        internal const string DeviceId = "mouse";

        public event DeviceConnectedHandler Connected;
        public event DeviceDisconnectedHandler Disconnected;
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (value != enabled)
                {
                    enabled = value;
                    if (enabled)
                    {
                        device.Start();
                    }
                    else
                    {
                        device.Stop();
                    }
                }
            }
        }

        private readonly MouseHook hook;
        private readonly MouseDevice device;
        private bool enabled = false;
        private bool disposed = false;

        [ResolverMethod]
        public MouseDeviceProvider(InputConfigManager inputConfigManager, MouseHook hook)
        {
            this.hook = hook;
            device = new MouseDevice(inputConfigManager, hook);
            var config = inputConfigManager.LoadConfig(device);
            device.InputConfiguration = config;
            if (config.Autostart)
            {
                enabled = true;
                device.Start();
            }
            Connected?.Invoke(this, new DeviceConnectedEventArgs(device));
        }

        public void SearchDevices()
        {

        }

        public IEnumerable<IInputDevice> GetActiveDevices()
        {
            return new IInputDevice[] { device };
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                device.Dispose();
                hook.Dispose();
            }
            disposed = true;
        }
    }
}
