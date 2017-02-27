using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XOutput.Input.DirectInput;
using XOutput.Input.XInput;

namespace XOutput.Input.Mapper
{
    public class KeyboardToXInputMapper : InputMapperBase
    {
        /// <summary>
        /// Gets a new mapper from string value.
        /// </summary>
        /// <param name="text">Serialized mapper</param>
        /// <returns></returns>
        public static KeyboardToXInputMapper Parse(string text)
        {
            KeyboardToXInputMapper mapper = new KeyboardToXInputMapper();

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
                Key? input = null;
                if (!string.IsNullOrEmpty(values[1]))
                    input = (Key)Enum.Parse(typeof(Key), values[1]);
                var min = double.Parse(values[2]) / 100;
                var max = double.Parse(values[3]) / 100;
                mapper.SetMapping(key, new MapperData { InputType = input, MinValue = min, MaxValue = max });
            }

            return mapper;
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
