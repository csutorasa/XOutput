using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using XOutput.Common.Input;
using XOutput.DependencyInjection;
using XOutput.Mapping.Input;

namespace XOutput.Rest.Input
{
    public class InputsController : Controller
    {
        private readonly InputDevices inputDevices;

        [ResolverMethod]
        public InputsController(InputDevices inputDeviceManager)
        {
            this.inputDevices = inputDeviceManager;
        }

        [HttpGet]
        [Route("/api/inputs")]
        public ActionResult<IEnumerable<InputDeviceInfo>> ListInputDevices()
        {
            return inputDevices.FindAll().Select(d => new InputDeviceInfo
            {
                Id = d.Id,
                Name = d.Name,
                DeviceApi = d.DeviceApi.ToString(),
                Sources = d.FindAllSources().Select(s => new InputDeviceSource {
                    Id = s.Id,
                    Name = s.Name,
                    Type = s.Type.ToString(),
                }).ToList(),
                Targets = d.FindAllTargets().Select(t => new InputDeviceTarget {
                    Id = t.Id,
                    Name = t.Name,
                    Type = "ForceFeedback",
                }).ToList(),
            }).ToList();
        }

        [HttpGet]
        [Route("/api/inputs/{id}")]
        public ActionResult<InputDeviceInfo> GetInputDevice(string id)
        {
            var d = inputDevices.Find(id);
            if (d == null) {
                return NotFound();
            }
            return new InputDeviceInfo {
                Id = d.Id,
                Name = d.Name,
                DeviceApi = d.DeviceApi.ToString(),
                Sources = d.FindAllSources().Select(s => new InputDeviceSource {
                    Id = s.Id,
                    Name = s.Name,
                    Type = s.Type.ToString(),
                }).ToList(),
                Targets = d.FindAllTargets().Select(t => new InputDeviceTarget {
                    Id = t.Id,
                    Name = t.Name,
                    Type = "ForceFeedback",
                }).ToList(),
            };
        }
    }
}