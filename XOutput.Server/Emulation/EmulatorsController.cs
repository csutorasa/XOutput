using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XOutput.Api.Devices;
using XOutput.Api.Emulation;
using XOutput.Core.DependencyInjection;

namespace XOutput.Server.Emulation {
    public class EmulatorsController : Controller
    {
        private readonly EmulatorService emulatorService;
        private readonly DeviceInfoService deviceInfoService;

        [ResolverMethod]
        public EmulatorsController(EmulatorService emulatorService, DeviceInfoService deviceInfoService)
        {
            this.emulatorService = emulatorService;
            this.deviceInfoService = deviceInfoService;
        }

        [HttpGet]
        [Route("/api/emulators")]
        public ActionResult<Dictionary<string, EmulatorResponse>> ListEmulators()
        {
            var emulators = emulatorService.GetEmulators();

            return emulators.ToDictionary(e => e.Name, e => new EmulatorResponse
            {
                Installed = e.Installed,
                SupportedDeviceTypes = e.SupportedDeviceTypes.Select(x => x.ToString()).ToList()
            });
        }

        [HttpGet]
        [Route("/api/controllers")]
        public ActionResult<IEnumerable<Api.Devices.DeviceInfo>> ListActiveControllers()
        {
            var devices = deviceInfoService.GetConnectedDevices();
            return devices.Select(d => new DeviceInfo
            {
                Id = d.Device.Id,
                Address = d.IPAddress,
                DeviceType = d.DeviceType.ToString(),
                Emulator = d.Emulator,
                Active = true,
                Local = false,
            }).ToList();
        }

        [HttpDelete]
        [Route("/api/controllers/{id}")]
        public Task DeleteDevice(string id)
        {
            deviceInfoService.StopAndRemove(id);
            Response.StatusCode = 204;
            return Task.CompletedTask;
        }
    }
}