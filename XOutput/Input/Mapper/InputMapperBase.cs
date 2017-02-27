using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input.XInput;

namespace XOutput.Input.Mapper
{
    public abstract class InputMapperBase
    {
        protected readonly Dictionary<XInputTypes, MapperData> mappings = new Dictionary<XInputTypes, MapperData>();

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
                mappings[type.Value] = new MapperData();
            }
            return mappings[type.Value];
        }
    }
}
