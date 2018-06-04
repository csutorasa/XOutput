using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices.Input.DirectInput
{
    /// <summary>
    /// General DirectInput device manager class.
    /// </summary>
    public sealed class DirectInputDevices : IDisposable
    {
        /// <summary>
        /// Id of the emulated SCP device
        /// </summary>
        private const string EmulatedSCPID = "028e045e-0000-0000-0000-504944564944";

        public bool AllDevices
        {
            get => allDevices;
            set
            {
                if (value != allDevices)
                {
                    allDevices = value;
                    RefreshInputDevices();
                }
            }
        }

        public event DeviceConnectedHandler DeviceConnected;
        public IEnumerable<DirectDevice> ConnectedDevices => connectedDevices;

        private bool allDevices = false;
        private IEnumerable<DirectDevice> connectedDevices = new DirectDevice[0];

        private readonly SharpDX.DirectInput.DirectInput directInput = new SharpDX.DirectInput.DirectInput();

        ~DirectInputDevices()
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
        public void RefreshInputDevices()
        {
            IEnumerable<DeviceInstance> newConnectedDevices;
            if (AllDevices)
            {
                newConnectedDevices = directInput.GetDevices().Where(di => di.Type != DeviceType.Keyboard).ToArray();
            }
            else
            {
                newConnectedDevices = directInput.GetDevices().Where(di => di.Type == DeviceType.Joystick || di.Type == DeviceType.Gamepad).ToArray();
            }
            var newDevices = newConnectedDevices.Where(d => !connectedDevices.Any(c => c.Id == d.InstanceGuid)).Select(CreateDirectDevice).Where(d => d != null);
            var removedDevices = connectedDevices.Where(c => !newConnectedDevices.Any(d => c.Id == d.InstanceGuid));
            foreach (var newDevice in newDevices)
            {
                DeviceConnected?.Invoke(newDevice, new DeviceConnectedEventArgs());
            }
            foreach (var removeDevice in removedDevices)
            {
                removeDevice.Dispose();
            }

        }

        /// <summary>
        /// Creates a wrapper for the native <paramref name="deviceInstance"/>.
        /// </summary>
        /// <param name="deviceInstance">native instance</param>
        /// <returns>Wrapped instance</returns>
        public DirectDevice CreateDirectDevice(DeviceInstance deviceInstance)
        {
            try
            {
                var joystick = new Joystick(directInput, deviceInstance.InstanceGuid);
                if (joystick.Information.ProductGuid.ToString() == EmulatedSCPID || (joystick.Capabilities.AxeCount < 1 && joystick.Capabilities.ButtonCount < 1))
                {
                    joystick.Dispose();
                    return null;
                }
                joystick.Properties.BufferSize = 128;

                return new DirectDevice(deviceInstance, joystick);
            }
            catch
            {
                return null;
            }
        }
    }
}
