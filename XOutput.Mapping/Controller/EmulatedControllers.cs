using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Common;
using XOutput.DependencyInjection;
using XOutput.Mapping.Controller.Ds4;
using XOutput.Mapping.Controller.Xbox;

namespace XOutput.Mapping.Controller
{
    public class EmulatedControllers
    {
        private readonly List<IEmulatedController> controllers = new List<IEmulatedController>();
        private readonly object sync = new object();

        [ResolverMethod]
        public EmulatedControllers()
        {

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
