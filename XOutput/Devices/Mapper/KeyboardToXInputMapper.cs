using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XOutput.Devices.Mapper
{
    /// <summary>
    /// Mapper that converts keyboard into XInput
    /// </summary>
    public class KeyboardToXInputMapper : InputMapperBase
    {
        /// <summary>
        /// Gets a new mapper from dictionary.
        /// </summary>
        /// <param name="data">Serialized mapper</param>
        /// <returns></returns>
        public static KeyboardToXInputMapper Parse(Dictionary<string, string> data)
        {
            KeyboardToXInputMapper mapper = new KeyboardToXInputMapper();
            mapper.StartWhenConnected = ReadStartWhenConnected(data);
            foreach (var mapping in FromDictionary(data))
            {
                mapper.mappings.Add(mapping.Key, mapping.Value);
            }
            return mapper;
        }
    }
}
