using NLog;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;

namespace XOutput.Devices.Input.Keyboard
{
    public sealed class KeyboardDeviceProvider : InputConfigManager, IInputDeviceProvider
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        internal const string DeviceId = "keyboard";

        public event DeviceConnectedHandler Connected;
        public event DeviceDisconnectedHandler Disconnected;

        private readonly KeyboardHook hook;
        private bool disposed = false;
        private KeyboardDevice device;

        [ResolverMethod]
        public KeyboardDeviceProvider(ConfigurationManager configurationManager, KeyboardHook hook) : base(configurationManager)
        {
            this.hook = hook;
#if !DEBUG
            hook.StartHook();
#endif
        }

        public void SearchDevices()
        {
            if(device == null)
            {
                device = new KeyboardDevice(hook);
                var config = LoadConfig(DeviceId);
                device = new KeyboardDevice(hook)
                {
                    InputConfiguration = config,
                };
                Connected?.Invoke(this, new DeviceConnectedEventArgs(device));
            }
        }

        public IEnumerable<IInputDevice> GetActiveDevices()
        {
            if (device == null)
            {
                return new IInputDevice[] { };
            }
            return new IInputDevice[] { device };
        }

        public bool SaveInputConfig(string id, InputConfig config)
        {
            if (DeviceId != id || device == null)
            {
                return false;
            }
            SaveConfig(DeviceId, config);
            device.InputConfiguration = config;
            return true;
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
                if(device != null)
                {
                    device.Dispose();
                }
                hook.Dispose();
            }
            disposed = true;
        }
    }
}
