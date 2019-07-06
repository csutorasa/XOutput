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
        /// Starts the mapping when connected.
        /// </summary>
        public bool StartWhenConnected { get; set; }
        /// <summary>
        /// Name of the mapper.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id of the mapper.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Id of the force feedback device.
        /// </summary>
        public string ForceFeedbackDevice { get; set; }

        public Dictionary<XInputTypes, MapperData> Mappings { get; set; }

        private ISet<IInputDevice> inputs = new HashSet<IInputDevice>();

        public InputMapper()
        {
            Mappings = new Dictionary<XInputTypes, MapperData>();
        }

        public ISet<IInputDevice> GetInputs()
        {
            return inputs;
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

        public void Attach(IEnumerable<IInputDevice> inputDevices)
        {
            inputs.Clear();
            foreach (var mapping in Mappings)
            {
                mapping.Value.Source = null;
                foreach (var inputDevice in inputDevices)
                {
                    if (mapping.Value.InputDevice != null && mapping.Value.InputType != null && mapping.Value.InputDevice == inputDevice.UniqueId)
                    {
                        mapping.Value.Source = inputDevice.Sources.FirstOrDefault(s => s.Offset.ToString() == mapping.Value.InputType);
                        inputs.Add(inputDevice);
                        break;
                    }
                }
            }
            foreach (var inputDevice in inputs)
            {
                inputDevice.RefreshInput(true);
            }
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
