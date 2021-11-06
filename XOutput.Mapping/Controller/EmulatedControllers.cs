using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Common;
using XOutput.DependencyInjection;
using XOutput.Emulation;
using XOutput.Emulation.Xbox;
using XOutput.Mapping.Controller.Ds4;
using XOutput.Mapping.Controller.Xbox;

namespace XOutput.Mapping.Controller
{
    public class EmulatedControllers
    {
        private readonly List<IEmulatedController> controllers = new List<IEmulatedController>();
        private readonly EmulatorService emulatorService;
        private readonly object sync = new object();

        [ResolverMethod]
        public EmulatedControllers(EmulatorService emulatorService)
        {
            this.emulatorService = emulatorService;
        }

        public IEmulatedController Create(string id, string name, DeviceTypes deviceType)
        {
            IEmulatedController controller = deviceType switch {
                DeviceTypes.MicrosoftXbox360 => new XboxController(),
                DeviceTypes.SonyDualShock4 => new Ds4Controller(),
                _ => throw new NotImplementedException($"Unknown device type {deviceType}"),
            };
            lock (sync)
            {
                controllers.Add(controller);
            }
            return controller;
        }

        public IEmulatedController Find(string id)
        {
            return controllers.FirstOrDefault(d => d.Id == id);
        }

        public void Start(IEmulatedController controller)
        {
            switch (controller.Device.DeviceType) {
                case DeviceTypes.MicrosoftXbox360:
                    (controller as XboxController).Start(emulatorService.FindBestXboxEmulator());
                    break;
                case DeviceTypes.SonyDualShock4:
                    (controller as Ds4Controller).Start(emulatorService.FindBestDs4Emulator());
                    break;
                default:
                    throw new ArgumentException("device.Device.DeviceType is not known");
            }
        }

        public void Stop(IEmulatedController controller)
        {
            controller.Stop();
        }

        public List<IEmulatedController> FindAll()
        {
            return controllers.ToList();
        }

        public bool Remove(IEmulatedController controller)
        {
            lock (sync)
            {
                return controllers.Remove(controller);
            }
        }
    }
}
