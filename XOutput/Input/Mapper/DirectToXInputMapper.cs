using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input.DirectInput;
using XOutput.Input.XInput;

namespace XOutput.Input.Mapper
{
    /// <summary>
    /// Converts DirectInput into XInput
    /// </summary>
    public sealed class DirectToXInputMapper
    {
        private readonly Dictionary<XInputTypes, MapperData<DirectInputTypes>> mappings = new Dictionary<XInputTypes, MapperData<DirectInputTypes>>();

        public DirectToXInputMapper()
        {

        }

        /// <summary>
        /// Gets a new mapper from string value.
        /// </summary>
        /// <param name="text">Serialized mapper</param>
        /// <returns></returns>
        public static DirectToXInputMapper Parse(string text)
        {
            DirectToXInputMapper mapper = new DirectToXInputMapper();

            foreach (var line in text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None))
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                var values = line.Split(';');
                if (values.Length != 4)
                {
                    throw new ArgumentException("Invalid text: " + text);
                }
                var key = (XInputTypes)Enum.Parse(typeof(XInputTypes), values[0]);
                DirectInputTypes? input = null;
                if (!string.IsNullOrEmpty(values[1]))
                    input = (DirectInputTypes)Enum.Parse(typeof(DirectInputTypes), values[1]);
                var min = double.Parse(values[2]) / 100;
                var max = double.Parse(values[3]) / 100;
                mapper.SetMapping(key, new MapperData<DirectInputTypes> { InputType = input, MinValue = min, MaxValue = max });
            }

            return mapper;
        }

        /// <summary>
        /// Sets the mapping for a given XInput.
        /// </summary>
        /// <param name="xInputType"></param>
        /// <param name="to"></param>
        public void SetMapping(XInputTypes xInputType, MapperData<DirectInputTypes> to)
        {
            mappings[xInputType] = to;
        }

        /// <summary>
        /// Gets the mapping for a given XInput. If the 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public MapperData<DirectInputTypes> GetMapping(XInputTypes? type)
        {
            if (!type.HasValue)
                return null;
            if(!mappings.ContainsKey(type.Value))
            {
                mappings[type.Value] = new MapperData<DirectInputTypes>();
            }
            return mappings[type.Value];
        }

        /// <summary>
        /// Serializes the mapper object into string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var mapping in mappings)
            {
                sb.Append(mapping.Key);
                sb.Append(";");
                sb.Append(mapping.Value.InputType);
                sb.Append(";");
                sb.Append((int)Math.Round(mapping.Value.MinValue * 100));
                sb.Append(";");
                sb.Append((int)Math.Round(mapping.Value.MaxValue * 100));
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
