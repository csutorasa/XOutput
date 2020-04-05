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
        public IEnumerable<int> ChangedDPads => changedDPads;

        protected IInputDevice device;
        protected IEnumerable<InputSource> changedValues;
        protected IEnumerable<int> changedDPads;

        public DeviceInputChangedEventArgs(IInputDevice device)
        {
            this.device = device;
            changedValues = new InputSource[0];
            changedDPads = new int[0];
        }

        public void Refresh(IEnumerable<InputSource> changedValues)
        {
            this.changedValues = changedValues;
        }

        public void Refresh(IEnumerable<int> changedDPads)
        {
            this.changedDPads = changedDPads;
        }

        public void Refresh(IEnumerable<InputSource> changedValues, IEnumerable<int> changedDPads)
        {
            this.changedDPads = changedDPads;
            this.changedValues = changedValues;
        }

        public bool HasValueChanged(InputSource type)
        {
            return changedValues.Contains(type);
        }

        public bool HasDPadChanged(int dPadIndex)
        {
            return changedDPads.Contains(dPadIndex);
        }
    }
}
