using System;
using System.Collections.Generic;
using System.Threading;
using XOutput.Common;
using XOutput.DependencyInjection;
using XOutput.Emulation.Xbox;

namespace XOutput.Emulation.SCPToolkit
{
    public class ScpEmulator : IXboxEmulator
    {
        public bool Installed { get; private set; }

        public Emulators Emulator => Emulators.SCPToolkit;

        public IEnumerable<DeviceTypes> SupportedDeviceTypes { get; } = new DeviceTypes[] { DeviceTypes.MicrosoftXbox360 };

        private int counter = 0;
        private ScpClient client;

        [ResolverMethod]
        public ScpEmulator()
        {
            Installed = Initialize();
        }

        public XboxDevice CreateXboxDevice()
        {
            int controllerIndex = Interlocked.Increment(ref counter);
            return new ScpDevice(controllerIndex, client);
        }

        public void Close()
        {
            Installed = false;
            client.UnplugAll();
            client.Dispose();
        }

        private bool Initialize()
        {
            try
            {
                client = new ScpClient();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
