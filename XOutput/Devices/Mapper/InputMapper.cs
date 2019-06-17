using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.Input;
using XOutput.Devices.XInput;

namespace XOutput.Devices.Mapper
{
    /// <summary>
    /// Base of mappers.
    /// </summary>
    public class InputMapper
    {
        /// <summary>
        /// DPad index to use
        /// </summary>
        public int SelectedDPad { get; set; }

        /// <summary>
        /// Starts the mapping when connected.
        /// </summary>
        public bool StartWhenConnected { get; set; }

        public Dictionary<XInputTypes, MapperData> Mappings { get; set; }

        public InputMapper()
        {
            Mappings = new Dictionary<XInputTypes, MapperData>();
            SelectedDPad = -1;
        }

        /// <summary>
        /// Sets the mapping for a given XInput.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public void SetMapping(XInputTypes type, MapperData to)
        {
            Mappings[type] = to;
        }

        /// <summary>
        /// Gets the mapping for a given XInput. If the mapping does not exist, returns a new mapping.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public MapperData GetMapping(XInputTypes? type)
        {
            if (!type.HasValue)
                return null;
            if (!Mappings.ContainsKey(type.Value))
            {
                Mappings[type.Value] = new MapperData { InputType = null, MinValue = type.Value.GetDisableValue(), MaxValue = type.Value.GetDisableValue() };
            }
            return Mappings[type.Value];
        }

        public void Attach(IInputDevice inputDevice)
        {
            foreach (var mapping in Mappings)
            {
                if (mapping.Value.InputType != null)
                {
                    mapping.Value.Source = inputDevice.Sources.FirstOrDefault(s => s.Offset.ToString() == mapping.Value.InputType);
                }
            }
            inputDevice.RefreshInput(true);
        }

        /// <summary>
        /// Reads value from file if available.
        /// </summary>
        /// <param name="data">line read from the file</param>
        /// <param name="defaultValue">default value to be returned when no value can be read</param>
        /// <returns></returns>
        protected static double TryReadValue(string data, double defaultValue = 0)
        {
            double value;
            if (double.TryParse(data, out value))
            {
                return value / 100;
            }
            else
            {
                return defaultValue;
            }
        }
    }
}
