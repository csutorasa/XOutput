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
        private const string EXCLUSIVE = "Exclusive";
        private bool isExclusive;
        public override bool IsExclusive
        {
            get => isExclusive;
            set
            {
                if (isExclusive != value)
                {
                    isExclusive = value;
                }
            }
        }

        /// <summary>
        /// Gets a new mapper from dictionary.
        /// </summary>
        /// <param name="data">Serialized mapper</param>
        /// <returns></returns>
        public static DirectToXInputMapper Parse(Dictionary<string, string> data)
        {
            DirectToXInputMapper mapper = new DirectToXInputMapper();
            if(data.ContainsKey(EXCLUSIVE))
            {
                mapper.IsExclusive = data[EXCLUSIVE] == "true";
                data.Remove(EXCLUSIVE);
            }
            else
            {
                mapper.IsExclusive = false;
            }
            foreach (var mapping in FromDictionary(data, typeof(DirectInputTypes)))
            {
                mapper.mappings.Add(mapping.Key, mapping.Value);
            }
            return mapper;
        }

        public override Dictionary<string, string> ToDictionary()
        {
            var dict = base.ToDictionary();
            dict[EXCLUSIVE] = IsExclusive ? "true" : "false";
            return dict;
        }
    }
}
