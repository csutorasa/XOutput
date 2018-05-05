using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices
{
    public delegate void DeviceInputChangedHandler(object sender, DeviceInputChangedEventArgs e);

    public class DeviceInputChangedEventArgs : EventArgs
    {
        public IEnumerable<Enum> ChangedValues => changedValues.ToArray();
        public IEnumerable<int> ChangedDPads => changedDPads.ToArray();

        protected IEnumerable<Enum> changedValues;
        protected IEnumerable<int> changedDPads;

        public DeviceInputChangedEventArgs(IEnumerable<Enum> changedValues, IEnumerable<int> changedDPads)
        {
            this.changedDPads = changedDPads;
            this.changedValues = changedValues;
        }

        public bool HasValueChanged(Enum type)
        {
            return changedValues.Contains(type);
        }

        public bool HasDPadChanged(int dPadIndex)
        {
            return changedDPads.Contains(dPadIndex);
        }
    }
}
