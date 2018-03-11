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
    public sealed class DirectToXInputMapper : InputMapperBase
    {
        /// <summary>
        /// Gets a new mapper from dictionary.
        /// </summary>
        /// <param name="data">Serialized mapper</param>
        /// <returns></returns>
        public static DirectToXInputMapper Parse(Dictionary<string, string> data)
        {
            DirectToXInputMapper mapper = new DirectToXInputMapper();
            foreach(var mapping in FromDictionary(data, typeof(DirectInputTypes)))
            {
                mapper.mappings.Add(mapping.Key, mapping.Value);
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
