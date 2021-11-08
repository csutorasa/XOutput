using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using XOutput.DependencyInjection;
using XOutput.Emulation;

namespace XOutput.Rest.Emulation
{
    public class EmulatedControllersController : Controller
    {
        private readonly EmulatedControllersService emulatedControllersService;
        private readonly EmulatorService emulatorService;

        [ResolverMethod]
        public EmulatedControllersController(EmulatedControllersService networkDeviceInfoService, EmulatorService emulatorService)
        {
            this.emulatedControllersService = networkDeviceInfoService;
            this.emulatorService = emulatorService;
        }

        [HttpGet]
        [Route("/api/emulated/controllers")]
        public ActionResult<IEnumerable<EmulatedControllerInfo>> ListControllers()
        {
            return emulatedControllersService.GetConnectedDevices().Select(d => new EmulatedControllerInfo
            {
                Id = d.Device.Id,
                Address = d.IPAddress,
                Name = d.Device.Id,
                DeviceType = d.DeviceType.ToString(),
                Emulator = d.Emulator.ToString(),
            }).ToList();
        }

        [HttpDelete]
        [Route("/api/emulated/controllers/{id}")]
        public ActionResult DeleteDevice(string id)
        {
            bool deleted = emulatedControllersService.StopAndRemove(id);
            if (!deleted) {
                return NotFound($"Cannot find emulated controller with id {id}");
            }
            return StatusCode(204);
        }
    }
}