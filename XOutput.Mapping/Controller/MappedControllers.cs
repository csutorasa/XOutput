using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Common;
using XOutput.DependencyInjection;
using XOutput.Emulation;
using XOutput.Mapping.Controller.Ds4;
using XOutput.Mapping.Controller.Xbox;

namespace XOutput.Mapping.Controller
{
    public class MappedControllers
    {
        private readonly List<IMappedController> controllers = new List<IMappedController>();
        private readonly EmulatorService emulatorService;
        private readonly object sync = new object();

        [ResolverMethod]
        public MappedControllers(EmulatorService emulatorService)
        {
            this.emulatorService = emulatorService;
        }

        public IMappedController Create(string id, string name, DeviceTypes deviceType)
        {
            IMappedController controller = deviceType switch {
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

        public IMappedController Find(string id)
        {
            return controllers.FirstOrDefault(d => d.Id == id);
        }

        public void Start(IMappedController controller)
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

        public void Stop(IMappedController controller)
        {
            controller.Stop();
        }

        public List<IMappedController> FindAll()
        {
            return controllers.ToList();
        }

        public bool Remove(IMappedController controller)
        {
            lock (sync)
            {
                return controllers.Remove(controller);
            }
        }
    }
}
