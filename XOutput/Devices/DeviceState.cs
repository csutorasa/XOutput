using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Devices
{
    /// <summary>
    /// Holds a current state of the device.
    /// </summary>
    public class DeviceState
    {
        /// <summary>
        /// Gets the current values.
        /// </summary>
        public Dictionary<InputType, double> Values => values;
        /// <summary>
        /// Gets the current DPad values.
        /// </summary>
        public IEnumerable<DPadDirection> DPads => dPads;
        protected readonly InputType[] valueTypes;
        protected readonly Dictionary<InputType, double> values = new Dictionary<InputType, double>();
        protected readonly DPadDirection[] dPads;
        protected readonly Func<InputType, double> typeGetter;
        protected readonly Func<int, DPadDirection> dPadGetter;
        protected readonly ICollection<int> changedDPads = new HashSet<int>();
        protected readonly ICollection<InputType> changedValues = new HashSet<InputType>();

        public DeviceState(IEnumerable<InputType> types, int dPadCount, Func<InputType, double> typeGetter, Func<int, DPadDirection> dPadGetter)
        {
            foreach (InputType type in types)
            {
                values.Add(type, 0);
            }
            dPads = new DPadDirection[dPadCount];
            this.typeGetter = typeGetter;
            this.dPadGetter = dPadGetter;
            valueTypes = values.Keys.ToArray();
        }

        /// <summary>
        /// Sets new DPad values.
        /// </summary>
        /// <returns>changed DPad indices</returns>
        public IEnumerable<int> SetDPads()
        {
            changedDPads.Clear();
            for (int i = 0; i < dPads.Length; i++)
            {
                DPadDirection newValue = dPadGetter(i);
                if (dPads[i] != newValue)
                {
                    dPads[i] = newValue;
                    changedDPads.Add(i);
                }
            }
            return changedDPads;
        }

        /// <summary>
        /// Sets new values.
        /// </summary>
        /// <returns>changed value types</returns>
        public IEnumerable<InputType> SetValues()
        {
            changedValues.Clear();
            foreach (var key in valueTypes)
            {
                double newValue = typeGetter(key);
                double oldValue = values[key];
                if (oldValue != newValue)
                {
                    values[key] = newValue;
                    changedValues.Add(key);
                }
            }
            return changedValues;
        }
    }
}
