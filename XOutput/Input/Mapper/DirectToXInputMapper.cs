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
            try
            {
                var selectedDPadText = data[SELECTEDDPAD_KEY];
                int selectedDPad = int.Parse(selectedDPadText);
                mapper.SelectedDPad = selectedDPad;
            }
            catch (Exception) { }
            foreach (var mapping in FromDictionary(data, typeof(DirectInputTypes)))
            {
                mapper.mappings.Add(mapping.Key, mapping.Value);
            }
            return mapper;
        }
    }
}
