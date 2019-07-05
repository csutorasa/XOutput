using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices
{
    /// <summary>
    /// Event delegate for DeviceInputChanged event.
    /// </summary>
    /// <param name="sender">the <see cref="IDevice"/> with changed input</param>
    /// <param name="e">event arguments</param>
    public delegate void DeviceInputChangedHandler(object sender, DeviceInputChangedEventArgs e);

    /// <summary>
    /// Event argument class for DeviceInputChanged event.
    /// </summary>
    public class DeviceInputChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the changed device.
        /// </summary>
        public IDevice Device => device;
        /// <summary>
        /// Gets the changed values.
        /// </summary>
        public IEnumerable<InputSource> ChangedValues => changedValues.ToArray();
        /// <summary>
        /// Gets the changed DPad values.
        /// </summary>
        public IEnumerable<int> ChangedDPads => changedDPads.ToArray();

        protected IDevice device;
        protected IEnumerable<InputSource> changedValues;
        protected IEnumerable<int> changedDPads;

        public DeviceInputChangedEventArgs(IDevice device, IEnumerable<InputSource> changedValues, IEnumerable<int> changedDPads)
        {
            this.device = device;
            this.changedDPads = changedDPads;
            this.changedValues = changedValues;
        }

        /// <summary>
        /// Gets if the value of the type has changed.
        /// </summary>
        /// <param name="type">input type</param>
        /// <returns></returns>
        public bool HasValueChanged(InputSource type)
        {
            return changedValues.Contains(type);
        }

        /// <summary>
        /// Gets if the value of the DPad has changed.
        /// </summary>
        /// <param name="type">input type</param>
        /// <returns></returns>
        public bool HasDPadChanged(int dPadIndex)
        {
            return changedDPads.Contains(dPadIndex);
        }
    }
}
