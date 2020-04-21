using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using XOutput.Api.Input;
using XOutput.Core.DependencyInjection;
using XOutput.Devices;
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

        [HttpGet]
        [Route("/api/inputs/{id}")]
        public ActionResult<InputDeviceDetails> InputDeviceDetails(string id)
        {
            var inputDevice = inputDeviceManager.FindInputDevice(id);
            if (inputDevice == null)
            {
                return NotFound();
            }
            return new InputDeviceDetails
            {
                Id = inputDevice.UniqueId,
                Name = inputDevice.DisplayName,
                HardwareId = inputDevice.HardwareID,
                Sources = inputDevice.Sources.Select(s => new InputDeviceSource { Offset = s.Offset, Name = s.DisplayName, Type = fromType(s.Type), }).ToList(),
                ForceFeedbacks = inputDevice.ForceFeedbacks.Select(s => new InputForceFeedback { Offset = s.Offset }).ToList(),
            };
        }

        [HttpPost]
        [Route("/api/inputs/{id}/forcefeedback/{offset}")]
        public ActionResult<InputDeviceDetails> StartForceFeedback(string id, int offset)
        {
            var inputDevice = inputDeviceManager.FindInputDevice(id);
            if (inputDevice == null)
            {
                return NotFound("Device not found");
            }
            var target = inputDevice.FindTarget(offset);
            if (target == null)
            {
                return NotFound("ForceFeedback target not found");
            }
            target.Value = 1;
            return NoContent();
        }

        [HttpDelete]
        [Route("/api/inputs/{id}/forcefeedback/{offset}")]
        public ActionResult<InputDeviceDetails> StopForceFeedback(string id, int offset)
        {
            var inputDevice = inputDeviceManager.FindInputDevice(id);
            if (inputDevice == null)
            {
                return NotFound("Device not found");
            }
            var target = inputDevice.FindTarget(offset);
            if (target == null)
            {
                return NotFound("ForceFeedback target not found");
            }
            target.Value = 0;
            return NoContent();
        }

        private string fromType(SourceTypes sourceTypes)
        {
            if (sourceTypes.IsAxis())
            {
                return "axis";
            }
            if (sourceTypes == SourceTypes.Button)
            {
                return "button";
            }
            if (sourceTypes == SourceTypes.Dpad)
            {
                return "dpad";
            }
            if (sourceTypes == SourceTypes.Slider)
            {
                return "slider";
            }
            return null;
        }
    }
}