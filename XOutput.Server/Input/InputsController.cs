using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using XOutput.Api.Devices;
using XOutput.Api.Input;
using XOutput.Core.DependencyInjection;
using XOutput.Devices;
using XOutput.Devices.Input;
using XOutput.Server.Emulation.HidGuardian;

namespace XOutput.Server.Input
{
    public class InputsController : Controller
    {
        private readonly InputDeviceManager inputDeviceManager;
        private readonly HidGuardianManager hidGuardianManager;

        [ResolverMethod]
        public InputsController(InputDeviceManager inputDeviceManager, HidGuardianManager hidGuardianManager)
        {
            this.inputDeviceManager = inputDeviceManager;
            this.hidGuardianManager = hidGuardianManager;
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
                ActiveFeatures = d.GetActiveFeatures(),
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
                Inputs = inputDevice.GetInputDevices().Select(d => new InputDeviceInputDetails {
                    Running = d.Running,
                    Sources = d.Sources.Select(s => new InputDeviceSource { 
                        Offset = s.Offset,
                        Name = s.DisplayName,
                        Type = FromType(s.Type),
                    }).ToList(),
                    ForceFeedbacks = d.ForceFeedbacks.Select(s => new InputForceFeedback {
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

        [HttpGet]
        [Route("/api/inputs/{id}/hidguardian")]
        public ActionResult<HidGuardianInfo> GetHidGuardian(string id)
        {
            var inputDevice = inputDeviceManager.FindInputDevice(id);
            if (inputDevice == null)
            {
                return NotFound("Device not found");
            }
            return new HidGuardianInfo {
                Available = inputDevice.HardwareID != null && hidGuardianManager.Installed,
                Active = inputDevice.HardwareID != null && hidGuardianManager.IsAffected(inputDevice.HardwareID),
            };
        }

        [HttpPut]
        [Route("/api/inputs/{id}/hidguardian")]
        public ActionResult EnableHidGuardian(string id)
        {
            var inputDevice = inputDeviceManager.FindInputDevice(id);
            if (inputDevice == null)
            {
                return NotFound("Device not found");
            }
            if (inputDevice.HardwareID == null)
            {
                return NotFound("Hardware ID not found");
            }
            hidGuardianManager.AddAffectedDevice(inputDevice.HardwareID);
            return NoContent();
        }

        [HttpDelete]
        [Route("/api/inputs/{id}/hidguardian")]
        public ActionResult DisableHidGuardian(string id)
        {
            var inputDevice = inputDeviceManager.FindInputDevice(id);
            if (inputDevice == null)
            {
                return NotFound("Device not found");
            }
            if (inputDevice.HardwareID == null)
            {
                return NotFound("Hardware ID not found");
            }
            hidGuardianManager.RemoveAffectedDevice(inputDevice.HardwareID);
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
        }
    }
}