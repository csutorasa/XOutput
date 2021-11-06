using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XOutput.Common;
using XOutput.DependencyInjection;
using XOutput.Emulation;
using XOutput.Mapping.Controller;

namespace XOutput.Rest.Emulation
{
    public class ControllersController : Controller
    {
        private readonly NetworkDeviceInfoService networkDeviceInfoService;
        private readonly EmulatedControllers emulatedControllers;
        private readonly EmulatorService emulatorService;

        [ResolverMethod]
        public ControllersController(NetworkDeviceInfoService networkDeviceInfoService, EmulatedControllers emulatedControllers, EmulatorService emulatorService)
        {
            this.networkDeviceInfoService = networkDeviceInfoService;
            this.emulatedControllers = emulatedControllers;
            this.emulatorService = emulatorService;
        }

        [HttpGet]
        [Route("/api/controllers")]
        public ActionResult<IEnumerable<ControllerInfo>> ListControllers()
        {
            var networkDevices = networkDeviceInfoService.GetConnectedDevices().Select(d => new ControllerInfo
            {
                Id = d.Device.Id,
                Address = d.IPAddress,
                Name = d.Device.Id,
                DeviceType = d.DeviceType.ToString(),
                Emulator = d.Emulator.ToString(),
                Active = true,
            });
            var mappedDevices = emulatedControllers.FindAll().Select(c => new ControllerInfo
            {
                Id = c.Device.Id,
                Name = c.Name,
                DeviceType = c.Device == null ? null : c.Device.DeviceType.ToString(),
                Emulator = c.Device == null ? null : c.Device.Emulator.ToString(),
                Active = c.Device != null,
            });
            return networkDevices.Concat(mappedDevices).ToList();
        }

        [HttpPut]
        [Route("/api/controllers")]
        public ActionResult CreateController([FromBody] CreateControllerRequest request)
        {
            DeviceTypes deviceType;
            if (!Enum.TryParse<DeviceTypes>(request.DeviceType, false, out deviceType)) {
                return StatusCode(422, $"Failed to parse {request.DeviceType} into DeviceTypes.");
            }
            var mappedDevices = emulatedControllers.Create(Guid.NewGuid().ToString(), request.Name, deviceType);
            return StatusCode(204);
        }

        [HttpPut]
        [Route("/api/controllers/{id}/active")]
        public ActionResult StartController(string id)
        {
            var controller = emulatedControllers.Find(id);
            if (controller == null) {
                return NotFound($"Cannot find controller with id {id}");
            }
            emulatedControllers.Start(controller);
            return StatusCode(204);
        }

        [HttpDelete]
        [Route("/api/controllers/{id}/active")]
        public ActionResult StopController(string id)
        {
            var controller = emulatedControllers.Find(id);
            if (controller == null) {
                return NotFound($"Cannot find controller with id {id}");
            }
            emulatedControllers.Stop(controller);
            return StatusCode(204);
        }

        [HttpDelete]
        [Route("/api/controllers/{id}")]
        public ActionResult DeleteDevice(string id)
        {
            bool deleted = networkDeviceInfoService.StopAndRemove(id);
            if (!deleted) {
                var controller = emulatedControllers.Find(id);
                if (controller == null) {
                    return NotFound($"Cannot find controller with id {id}");
                } else {
                    emulatedControllers.Remove(controller);
                }
            }
            return StatusCode(204);
        }
    }
}