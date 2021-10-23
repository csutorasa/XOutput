using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XOutput.DependencyInjection;
using XOutput.Emulation;
using XOutput.Mapping.Input;
using XOutput.Rest.Devices;

namespace XOutput.Rest.Emulation
{
    public class ControllersController : Controller
    {
        private readonly NetworkDeviceInfoService networkDeviceInfoService;
        private readonly InputDevices InputDevices;

        [ResolverMethod]
        public ControllersController(NetworkDeviceInfoService networkDeviceInfoService, InputDevices InputDevices)
        {
            this.networkDeviceInfoService = networkDeviceInfoService;
            this.InputDevices = InputDevices;
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
            return networkDevices.ToList();
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