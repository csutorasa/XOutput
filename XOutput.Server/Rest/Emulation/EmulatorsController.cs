using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using XOutput.DependencyInjection;
using XOutput.Emulation;

namespace XOutput.Rest.Emulation
{
    public class EmulatorsController : Controller
    {
        private readonly EmulatorService emulatorService;

        [ResolverMethod]
        public EmulatorsController(EmulatorService emulatorService)
        {
            this.emulatorService = emulatorService;
        }

        [HttpGet]
        [Route("/api/emulators")]
        public ActionResult<Dictionary<string, EmulatorResponse>> ListEmulators()
        {
            var emulators = emulatorService.GetEmulators();

            return emulators.ToDictionary(e => e.Emulator.ToString(), e => new EmulatorResponse
            {
                Installed = e.Installed,
                SupportedDeviceTypes = e.SupportedDeviceTypes.Select(x => x.ToString()).ToList()
            });
        }
    }
}