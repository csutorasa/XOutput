using System;
using System.Collections.Generic;
using System.Linq;

namespace XOutput.Devices.Input
{
    public class InputDeviceHolder
    {
        public string DisplayName => displayName;
        public string InterfacePath => interfacePath;
        public string UniqueId => uniqueId;
        public string HardwareID => hardwareID;
        private readonly Dictionary<InputDeviceMethod, IInputDevice> devices = new Dictionary<InputDeviceMethod, IInputDevice>();
        private readonly string displayName;
        private readonly string interfacePath;
        private readonly string uniqueId;
        private readonly string hardwareID;

        public InputDeviceHolder(string displayName, string interfacePath, string uniqueId, string hardwareID)
        {
            this.displayName = displayName;
            this.interfacePath = interfacePath;
            this.uniqueId = uniqueId;
            this.hardwareID = hardwareID;
        }

        public IInputDevice FindInput(InputDeviceMethod method)
        {
            return devices[method];
        }

        public List<IInputDevice> GetInputDevices()
        {
            return devices.Values.ToList();
        }

        public void SetInput(InputDeviceMethod method, IInputDevice device)
        {
            devices[method] = device;
        }

        public bool RemoveInput(InputDeviceMethod method)
        {
            devices.Remove(method);
            return !devices.Any();
        }

        public List<string> GetActiveFeatures()
        {
            return devices.Where(d => d.Value.Running).Select(d => d.Key.ToString()).ToList();
        }

        public InputSource FindSource(InputDeviceMethod method, int offset)
        {
            return FindInput(method)?.FindSource(offset);
        }

        public ForceFeedbackTarget FindTarget(InputDeviceMethod method, int offset)
        {
            return FindInput(method)?.FindTarget(offset);
        }

        public void SetForceFeedback(InputDeviceMethod method, int offset, double value)
        {
            var target = FindInput(method)?.FindTarget(offset);
            if (target != null)
            {
                target.Value = value;
            }
        }
    }
}
