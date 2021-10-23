using Microsoft.AspNetCore.Mvc;
using XOutput.DependencyInjection;
using XOutput.Mapping.Input;

namespace XOutput.Rest.Input
{
    public class InputsController : Controller
    {
        private readonly MappableDevices inputDeviceManager;

        [ResolverMethod]
        public InputsController(MappableDevices inputDeviceManager)
        {
            this.inputDeviceManager = inputDeviceManager;
        }

        /*[HttpGet]
        [Route("/api/inputs")]
        public ActionResult<IEnumerable<InputDeviceInfo>> ListInputDevices()
        {
            var inputDevices = inputDeviceManager.GetInputDevices();

            return inputDevices.Select(d => new InputDeviceInfo
            {
                Id = d.UniqueId,
                Name = d.DisplayName,
                ActiveFeatures = d.GetActiveFeatures(),
            }).ToList();
        }

        [HttpGet]
        [Route("/api/inputs/{id}")]
        public ActionResult<InputDeviceDetailsMessage> InputDeviceDetails(string id)
        {
            var inputDevice = inputDeviceManager.FindInputDevice(id);
            if (inputDevice == null)
            {
                return NotFound();
            }
            return new InputDeviceDetailsMessage
            {
                Id = inputDevice.UniqueId,
                Name = inputDevice.DisplayName,
                HardwareId = inputDevice.HardwareID,
                Inputs = inputDevice.GetInputDevices().Select(d => new InputDeviceInputDetails
                {
                    Running = d.Running,
                    Sources = d.Sources.Select(s => new InputDeviceSource
                    {
                        Offset = s.Offset,
                        Name = s.DisplayName,
                        Type = FromType(s.Type),
                    }).ToList(),
                    ForceFeedbacks = d.ForceFeedbacks.Select(s => new InputForceFeedback
                    {
                        Offset = s.Offset
                    }).ToList(),
                    InputMethod = d.InputMethod.ToString(),
                }).ToList(),
            };
        }

        [HttpGet]
        [Route("/api/inputs/{id}/configuration")]
        public ActionResult<InputConfiguration> GetInputConfiguration(string id)
        {
            var inputDevice = inputDeviceManager.FindInputDevice(id);
            if (inputDevice == null)
            {
                return NotFound();
            }
            return new InputConfiguration
            {

            };
        }

        [HttpPost]
        [Route("/api/inputs/{id}/{method}/{offset}/deadzone")]
        public ActionResult SetDeadzone(string id, InputDeviceMethod method, int offset, [FromBody] double deadzone)
        {
            var device = inputDeviceManager.FindInputDevice(id);
            if (device == null)
            {
                return NotFound("Device not found");
            }
            var inputDevice = device.FindInput(method);
            if (inputDevice == null)
            {
                return NotFound("Input device method not found");
            }
            var source = inputDevice.FindSource(offset);
            if (source == null)
            {
                return NotFound("ForceFeedback target not found");
            }
            inputDeviceManager.ChangeInputConfiguration(id, method, (inputDevice) =>
            {
                var config = inputDevice.InputConfiguration.Sources.FirstOrDefault(s => s.Offset == offset);
                if (config == null)
                {
                    config = new InputSourceConfig
                    {
                        Offset = offset,
                    };
                    inputDevice.InputConfiguration.Sources.Add(config);
                }
                config.Deadzone = deadzone;
            });
            return NoContent();
        }

        [HttpPut]
        [Route("/api/inputs/{id}/forcefeedback/{offset}")]
        public ActionResult StartForceFeedback(string id, int offset)
        {
            var inputDevice = inputDeviceManager.FindInputDevice(id);
            if (inputDevice == null)
            {
                return NotFound("Device not found");
            }
            var target = inputDevice.FindInput(InputDeviceMethod.DirectInput)?.FindTarget(offset);
            if (target == null)
            {
                return NotFound("ForceFeedback target not found");
            }
            target.Value = 1;
            return NoContent();
        }

        [HttpDelete]
        [Route("/api/inputs/{id}/forcefeedback/{offset}")]
        public ActionResult StopForceFeedback(string id, int offset)
        {
            var inputDevice = inputDeviceManager.FindInputDevice(id);
            if (inputDevice == null)
            {
                return NotFound("Device not found");
            }
            var target = inputDevice.FindInput(InputDeviceMethod.DirectInput)?.FindTarget(offset);
            if (target == null)
            {
                return NotFound("ForceFeedback target not found");
            }
            target.Value = 0;
            return NoContent();
        }

        [HttpPut]
        [Route("/api/inputs/{id}/{method}/running")]
        public ActionResult StartInputDevice(string id, InputDeviceMethod method)
        {
            var inputDevice = inputDeviceManager.FindInputDevice(id);
            if (inputDevice == null)
            {
                return NotFound("Device not found");
            }
            var target = inputDevice.FindInput(method);
            if (target == null)
            {
                return NotFound("Input device not found");
            }
            target.Start();
            return NoContent();
        }

        [HttpDelete]
        [Route("/api/inputs/{id}/{method}/running")]
        public ActionResult StopForceFeedback(string id, InputDeviceMethod method)
        {
            var inputDevice = inputDeviceManager.FindInputDevice(id);
            if (inputDevice == null)
            {
                return NotFound("Device not found");
            }
            var target = inputDevice.FindInput(method);
            if (target == null)
            {
                return NotFound("Input device not found");
            }
            target.Stop();
            return NoContent();
        }

        private string FromType(SourceTypes sourceTypes)
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
        }*/
    }
}