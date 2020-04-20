using System;
using System.Collections.Generic;
using System.Linq;

namespace XOutput.Devices.Input
{
    public delegate void DeviceInputChangedHandler(object sender, DeviceInputChangedEventArgs e);

    public class DeviceInputChangedEventArgs : EventArgs
    {
        public IInputDevice Device => device;
        public IEnumerable<InputSource> ChangedValues => changedValues;

        protected IInputDevice device;
        protected IEnumerable<InputSource> changedValues;

        public DeviceInputChangedEventArgs(IInputDevice device)
        {
            this.device = device;
            changedValues = new InputSource[0];
        }

        public void Refresh(IEnumerable<InputSource> changedValues)
        {
            this.changedValues = changedValues;
        }

        public bool HasValueChanged(InputSource type)
        {
            return changedValues.Contains(type);
        }
    }
}
