using SlimDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Input.DirectInput
{

    public sealed class Devices : IDisposable
    {
        /// <summary>
        /// Id of the emulated SCP device
        /// </summary>
        private const string EMULATED_ID = "028e045e-0000-0000-0000-504944564944";
        
        private readonly SlimDX.DirectInput.DirectInput directInput = new SlimDX.DirectInput.DirectInput();

        public Devices()
        {

        }
        ~Devices()
        {
            Dispose();
        }

        /// <summary>
        /// Disposes all resources.
        /// </summary>
        public void Dispose()
        {
            directInput.Dispose();
        }

        /// <summary>
        /// Gets the current available DirectInput devices.
        /// </summary>
        /// <returns>List of devices</returns>
        public IEnumerable<DirectDevice> GetInputDevices()
        {
            var directDevices = new List<DirectDevice>();
            var deviceInstances = directInput.GetDevices(DeviceClass.GameController, DeviceEnumerationFlags.AttachedOnly);
            foreach(var deviceInstance in deviceInstances)
            {
                var joystick = new Joystick(directInput, deviceInstance.InstanceGuid);

                if (joystick.Information.ProductGuid.ToString() == EMULATED_ID || (joystick.Capabilities.AxesCount < 1 && joystick.Capabilities.ButtonCount < 1))
                {
                    joystick.Dispose();
                    continue;
                }

                joystick.Properties.BufferSize = 128;

                directDevices.Add(new DirectDevice(deviceInstance, joystick));
            }
            return directDevices;
        }
    }
}
