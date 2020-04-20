using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using XOutput.Api.Input;
using XOutput.Core.DependencyInjection;
using XOutput.Devices.Input;

namespace XOutput.Server.Input 
{
    public class InputsController : Controller
    {
        private readonly InputDeviceManager inputDeviceManager;

        [ResolverMethod]
        public InputsController(InputDeviceManager inputDeviceManager)
        {
            this.inputDeviceManager = inputDeviceManager;
        }

        [HttpGet]
        [Route("/api/inputs")]
        public ActionResult<IEnumerable<InputDeviceInfo>> ListInputDevices()
        {
            var inputDevices = inputDeviceManager.GetInputDevices();

            return inputDevices.Select(d => new InputDeviceInfo
            {
                Id = d.UniqueId,
                Name = d.DisplayName,
                Axes = d.Sources.Where(s => s.IsAxis).Count(),
                DPads = d.Sources.Where(s => s.IsDPad).Count() / 4,
                Buttons = d.Sources.Where(s => s.IsButton).Count(),
                Sliders = d.Sources.Where(s => s.IsSlider).Count(),
            }).ToList();
        }
    }
}