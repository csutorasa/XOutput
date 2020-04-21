using NLog;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Core.DependencyInjection;

namespace XOutput.Devices.Input.Mouse
{
    public sealed class MouseDeviceProvider : IInputDeviceProvider
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public event DeviceConnectedHandler Connected;
        public event DeviceDisconnectedHandler Disconnected;

        private readonly MouseHook hook;
        private bool disposed = false;
        private MouseDevice device;

        [ResolverMethod]
        public MouseDeviceProvider(MouseHook hook)
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
                device = new MouseDevice(hook);
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
