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
    public class InputMapperBase
    {
        /// <summary>
        /// Split char between values
        /// </summary>
        private const char SplitChar = ',';
        /// <summary>
        /// Selected DPad setting key
        /// </summary>
        protected const string SelectedDPadKey = "SelectedDPad";
        /// <summary>
        /// Start when connected key
        /// </summary>
        protected const string StartWhenConnectedKey = "StartWhenConnected";

        /// <summary>
        /// DPad index to use
        /// </summary>
        public int SelectedDPad { get; set; }

        /// <summary>
        /// Starts the mapping when connected.
        /// </summary>
        public bool StartWhenConnected { get; set; }

        public Dictionary<XInputTypes, MapperData> Mappings { get; set; }

        public InputMapperBase()
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
                    mapping.Value.Source = inputDevice.Sources.FirstOrDefault(s => s.DisplayName == mapping.Value.InputType);
                }
            }
        }

        /// <summary>
        /// Creates a dictionary to save the values to file.
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();
            dict.Add(SelectedDPadKey, SelectedDPad.ToString());
            dict.Add(StartWhenConnectedKey, StartWhenConnected ? "true" : "false");
            foreach (var mapping in Mappings)
            {
                dict.Add(mapping.Key.ToString(),
                    string.Join(SplitChar.ToString(), new string[] { mapping.Value.InputType?.ToString(), ((int)Math.Round(mapping.Value.MinValue * 100)).ToString(),
                        ((int)Math.Round(mapping.Value.MaxValue * 100)).ToString(), ((int)Math.Round(mapping.Value.Deadzone * 100)).ToString() }));
            }
            return dict;
        }

        /// <summary>
        /// Converts to mapping key-value pairs from the data read.
        /// </summary>
        /// <param name="data">Parsed file content</param>
        /// <returns></returns>
        protected static Dictionary<XInputTypes, MapperData> FromDictionary(Dictionary<string, string> data)
        {
            var dict = new Dictionary<XInputTypes, MapperData>();
            foreach (var mapping in data)
            {
                try
                {
                    XInputTypes key;
                    if (Enum.TryParse(mapping.Key, out key))
                    {
                        var values = mapping.Value.Split(SplitChar);
                        if (values.Length != 4)
                        {
                            throw new ArgumentException("Invalid text: " + mapping.Value);
                        }
                        string input = values[0];
                        double min = TryReadValue(values[1]);
                        double max = TryReadValue(values[2]);
                        double deadzone = TryReadValue(values[3]);
                        dict.Add(key, new MapperData { InputType = input, MinValue = min, MaxValue = max, Deadzone = deadzone });
                    }
                }
                catch
                {
                    // Ignored
                }
            }
            return dict;
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

        /// <summary>
        /// Read if Start when connected is set.
        /// </summary>
        /// <param name="data">data from the file</param>
        /// <returns></returns>
        protected static bool ReadStartWhenConnected(Dictionary<string, string> data)
        {
            try
            {
                string startWhenConnectedText = data[StartWhenConnectedKey];
                return startWhenConnectedText == "true";
            }
            catch
            {
                return false;
            }
        }
    }
}
