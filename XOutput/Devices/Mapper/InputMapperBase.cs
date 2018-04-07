using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.XInput;

namespace XOutput.Devices.Mapper
{
    public abstract class InputMapperBase
    {
        /// <summary>
        /// DPad index to use
        /// </summary>
        public int SelectedDPad { get; set; }

        /// <summary>
        /// Starts the mapping when connected.
        /// </summary>
        public bool StartWhenConnected { get; set; }

        private const char SplitChar = ',';
        protected const string SelectedDPadKey = "SelectedDPad";
        protected const string StartWhenConnectedKey = "StartWhenConnected";
        protected readonly Dictionary<XInputTypes, MapperData> mappings = new Dictionary<XInputTypes, MapperData>();

        public InputMapperBase()
        {
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
            mappings[type] = to;
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
            if (!mappings.ContainsKey(type.Value))
            {
                mappings[type.Value] = new MapperData { InputType = null, MinValue = type.Value.GetDisableValue(), MaxValue = type.Value.GetDisableValue() };
            }
            return mappings[type.Value];
        }

        public virtual Dictionary<string, string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();
            dict.Add(SelectedDPadKey, SelectedDPad.ToString());
            dict.Add(StartWhenConnectedKey, StartWhenConnected ? "true" : "false");
            foreach (var mapping in mappings)
            {
                dict.Add(mapping.Key.ToString(),
                    string.Join(SplitChar.ToString(), new string[] { mapping.Value.InputType?.ToString(), ((int)Math.Round(mapping.Value.MinValue * 100)).ToString(),
                        ((int)Math.Round(mapping.Value.MaxValue * 100)).ToString(), ((int)Math.Round(mapping.Value.Deadzone * 100)).ToString() }));
            }
            return dict;
        }

        protected static Dictionary<XInputTypes, MapperData> FromDictionary(Dictionary<string, string> data, Type enumType)
        {
            var dict = new Dictionary<XInputTypes, MapperData>();
            foreach (var mapping in data)
            {
                try
                {
                    var key = (XInputTypes)Enum.Parse(typeof(XInputTypes), mapping.Key);
                    var values = mapping.Value.Split(SplitChar);
                    if (values.Length != 4)
                    {
                        throw new ArgumentException("Invalid text: " + mapping.Value);
                    }
                    Enum input = null;
                    if (!string.IsNullOrEmpty(values[0]))
                        input = (Enum)Enum.Parse(enumType, values[0]);
                    double min = TryReadValue(values[1]);
                    double max = TryReadValue(values[2]);
                    double deadzone = TryReadValue(values[3]);
                    dict.Add(key, new MapperData { InputType = input, MinValue = min, MaxValue = max, Deadzone = deadzone });
                }
                catch
                {
                    // Ignored
                }
            }
            return dict;
        }

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
