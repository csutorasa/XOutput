using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.DependencyInjection;
using XOutput.Emulation;
using XOutput.Mapping.Controller;

namespace XOutput.Rest.Mapping
{
    public class MappedControllersController : Controller
    {
        private readonly MappedControllers mappedControllers;
        private readonly EmulatorService emulatorService;

        [ResolverMethod]
        public MappedControllersController(MappedControllers emulatedControllers, EmulatorService emulatorService)
        {
            this.mappedControllers = emulatedControllers;
            this.emulatorService = emulatorService;
        }

        [HttpGet]
        [Route("/api/mapped/controllers")]
        public ActionResult<IEnumerable<MappedControllerInfo>> ListControllers()
        {
            return mappedControllers.FindAll().Select(c => new MappedControllerInfo
            {
                Id = c.Device.Id,
                Name = c.Name,
                DeviceType = c.Device == null ? null : c.Device.DeviceType.ToString(),
                Emulator = c.Device == null ? null : c.Device.Emulator.ToString(),
                Active = c.Device != null,
            }).ToList();
        }

        [HttpPut]
        [Route("/api/mapped/controllers/{id}/active")]
        public ActionResult StartController(string id)
        {
            var controller = mappedControllers.Find(id);
            if (controller == null) {
                return NotFound($"Cannot find mapped controller with id {id}");
            }
            mappedControllers.Start(controller);
            return StatusCode(204);
        }

        [HttpDelete]
        [Route("/api/mapped/controllers/{id}/active")]
        public ActionResult StopController(string id)
        {
            var controller = mappedControllers.Find(id);
            if (controller == null) {
                return NotFound($"Cannot find mapped controller with id {id}");
            }
            mappedControllers.Stop(controller);
            return StatusCode(204);
        }

        [HttpDelete]
        [Route("/api/mapped/controllers/{id}")]
        public ActionResult DeleteDevice(string id)
        {
            var controller = mappedControllers.Find(id);
            if (controller == null) {
                return NotFound($"Cannot find mapped controller with id {id}");
            }
            mappedControllers.Remove(controller);
            return StatusCode(204);
        }
    }
}