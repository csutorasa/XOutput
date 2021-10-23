using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using System;
using System.Collections.Generic;
using XOutput.DependencyInjection;
using XOutput.Emulation.Ds4;
using XOutput.Emulation.Xbox;

namespace XOutput.Emulation.ViGEm
{
    public class ViGEmEmulator : IXboxEmulator, IDs4Emulator
    {
        public bool Installed { get; private set; }

        public string Name => "ViGEm";

        public IEnumerable<DeviceTypes> SupportedDeviceTypes { get; } = new DeviceTypes[] { DeviceTypes.MicrosoftXbox360, DeviceTypes.SonyDualShock4 };

        private ViGEmClient client;

        [ResolverMethod]
        public ViGEmEmulator()
        {
            try
            {
                Installed = Initialize();
            }
            catch
            {
                Installed = false;
            }
        }

        public XboxDevice CreateXboxDevice()
        {
            var controller = client.CreateXbox360Controller();
            return new ViGEmXboxDevice(controller);
        }

        public Ds4Device CreateDs4Device()
        {
            var controller = client.CreateDualShock4Controller();
            return new ViGEmDs4Device(controller);
        }

        public void Close()
        {
            Installed = false;
            client.Dispose();
        }

        private bool Initialize()
        {
            try
            {
                client = new ViGEmClient();
                return true;
            }
            catch (VigemBusNotFoundException)
            {
                return false;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
        }
    }
}
