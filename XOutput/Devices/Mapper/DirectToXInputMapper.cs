using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.Input.DirectInput;

namespace XOutput.Devices.Mapper
{
    /// <summary>
    /// Mapper that converts DirectInput into XInput
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
                string selectedDPadText = data[SelectedDPadKey];
                int selectedDPad = int.Parse(selectedDPadText);
                mapper.SelectedDPad = selectedDPad;
            }
            catch (Exception) { }
            mapper.StartWhenConnected = ReadStartWhenConnected(data);
            foreach (var mapping in FromDictionary(data, typeof(DirectInputTypes)))
            {
                mapper.mappings.Add(mapping.Key, mapping.Value);
            }
            return mapper;
        }
    }
}
