using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
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
        [Route("/api/devices")]
        public ActionResult<IEnumerable<Api.Devices.DeviceInfo>> ListActiveDevices()
        {
            var devices = deviceInfoService.GetConnectedDevices();
            return devices.Select(d => new DeviceInfo
            {
                Id = d.Device.Id,
                Address = d.IPAddress,
                DeviceType = d.DeviceType.ToString(),
                Emulator = d.Emulator,
                Active = true,
            }).ToList();
        }

        [HttpDelete]
        [Route("/api/devices/{id}")]
        public ActionResult DeleteDevice(string id)
        {
            deviceInfoService.StopAndRemove(id);
            return null;
        }
    }
}