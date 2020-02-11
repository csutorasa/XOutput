using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Exceptions;
using System;
using System.Collections.Generic;
using XOutput.Api.Devices;
using XOutput.Core.DependencyInjection;

namespace XOutput.Server.Emulation.ViGEm
{
    public class ViGEmEmulator : IXboxEmulator
    {
        public bool Installed { get; private set; }

        public string Name => "ViGEm";

        public IEnumerable<DeviceTypes> SupportedDeviceTypes { get; } = new DeviceTypes[] { DeviceTypes.MicrosoftXbox360 };

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

        public XboxDevice CreateDevice()
        {
            var controller = client.CreateXbox360Controller();
            return new ViGEmXboxDevice(controller);
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
