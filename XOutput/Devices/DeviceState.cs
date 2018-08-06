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
        protected Dictionary<InputType, double> values = new Dictionary<InputType, double>();
        protected DPadDirection[] dPads;

        public DeviceState(IEnumerable<InputType> types, int dPadCount)
        {
            foreach (InputType type in types)
            {
                values.Add(type, 0);
            }
            dPads = new DPadDirection[dPadCount];
        }

        /// <summary>
        /// Sets new DPad values.
        /// </summary>
        /// <param name="newDPads">new values</param>
        /// <returns>changed DPad indices</returns>
        public IEnumerable<int> SetDPads(IEnumerable<DPadDirection> newDPads)
        {
            if (newDPads.Count() != dPads.Length)
                throw new ArgumentException();
            ICollection<int> changed = new HashSet<int>();
            foreach (var x in newDPads.Select((d, i) => new { New = d, Old = dPads[i], Index = i }).ToArray())
            {
                if (x.New != x.Old)
                {
                    dPads[x.Index] = x.New;
                    changed.Add(x.Index);
                }
            }
            return changed;
        }

        /// <summary>
        /// Sets new values.
        /// </summary>
        /// <param name="newValues">new values</param>
        /// <returns>changed value types</returns>
        public IEnumerable<InputType> SetValues(Dictionary<InputType, double> newValues)
        {
            ICollection<InputType> changed = new HashSet<InputType>();
            foreach (var x in newValues.Select((d, i) => new { New = d.Value, Old = values[d.Key], Type = d.Key }).ToArray())
            {
                if (x.New != x.Old)
                {
                    values[x.Type] = x.New;
                    changed.Add(x.Type);
                }
            }
            return changed;
        }
    }
}
