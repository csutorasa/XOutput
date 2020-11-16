using System.Collections.Generic;
using System.Linq;
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

        public Dictionary<XInputTypes, MapperDataCollection> Mappings { get; set; }

        private readonly ISet<IInputDevice> inputs = new HashSet<IInputDevice>();

        public InputMapper()
        {
            Mappings = new Dictionary<XInputTypes, MapperDataCollection>();
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
        public void SetMapping(XInputTypes type, MapperDataCollection to)
        {
            Mappings[type] = to;
        }

        /// <summary>
        /// Gets the mapping for a given XInput. If the mapping does not exist, returns a new mapping.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public MapperDataCollection GetMapping(XInputTypes? type)
        {
            if (!type.HasValue)
            {
                return null;
            }
            if (!Mappings.ContainsKey(type.Value))
            {
                Mappings[type.Value] = new MapperDataCollection(new MapperData { Source = DisabledInputSource.Instance });
            }
            return Mappings[type.Value];
        }

        public void Attach(IEnumerable<IInputDevice> inputDevices)
        {
            inputs.Clear();
            foreach (var mapping in Mappings)
            {
                foreach (var mapperData in mapping.Value.Mappers)
                {
                    bool found = false;
                    if (mapperData.InputDevice != null && mapperData.InputType != null)
                    {
                        foreach (var inputDevice in inputDevices)
                        {
                            if (mapperData.InputDevice == inputDevice.UniqueId)
                            {
                                mapperData.SetSourceWithoutSaving(inputDevice.Sources.FirstOrDefault(s => s.Offset.ToString() == mapperData.InputType));
                                inputs.Add(inputDevice);
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        mapperData.SetSourceWithoutSaving(DisabledInputSource.Instance);
                    }
                }
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
