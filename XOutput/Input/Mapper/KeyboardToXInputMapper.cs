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
        /// Gets a new mapper from dictionary.
        /// </summary>
        /// <param name="data">Serialized mapper</param>
        /// <returns></returns>
        public static KeyboardToXInputMapper Parse(Dictionary<string, string> data)
        {
            KeyboardToXInputMapper mapper = new KeyboardToXInputMapper();
            foreach (var mapping in FromDictionary(data, typeof(Key)))
            {
                mapper.mappings.Add(mapping.Key, mapping.Value);
            }
            return mapper;
        }
    }
}
