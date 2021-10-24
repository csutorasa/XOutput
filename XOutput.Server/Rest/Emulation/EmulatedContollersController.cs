using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XOutput.DependencyInjection;
using XOutput.Emulation;
using XOutput.Mapping.Controller;
using XOutput.Rest.Devices;

namespace XOutput.Rest.Emulation
{
    public class ControllersController : Controller
    {
        private readonly NetworkDeviceInfoService networkDeviceInfoService;
        private readonly EmulatedControllers emulatedControllers;

        [ResolverMethod]
        public ControllersController(NetworkDeviceInfoService networkDeviceInfoService, EmulatedControllers emulatedControllers)
        {
            this.networkDeviceInfoService = networkDeviceInfoService;
            this.emulatedControllers = emulatedControllers;
        }

        [HttpGet]
        [Route("/api/controllers")]
        public ActionResult<IEnumerable<DeviceInfo>> ListControllers()
        {
            var networkDevices = networkDeviceInfoService.GetConnectedDevices().Select(d => new DeviceInfo
            {
                Id = d.Device.Id,
                Address = d.IPAddress,
                DeviceType = d.DeviceType.ToString(),
                Emulator = d.Emulator,
                Active = true,
            });
            var mappedDevices = emulatedControllers.FindAll().Select(c => new DeviceInfo
            {
                Id = c.Device.Id,
                DeviceType = c.Device == null ? null : c.Device.DeviceType.ToString(),
                Emulator = c.Device == null ? null : c.Device.Emulator.ToString(),
                Active = c.Device != null,
            });
            return networkDevices.Concat(mappedDevices).ToList();
        }

        [HttpDelete]
        [Route("/api/controllers/{id}")]
        public Task DeleteDevice(string id)
        {
            networkDeviceInfoService.StopAndRemove(id);
            Response.StatusCode = 204;
            return Task.CompletedTask;
        }
    }
}