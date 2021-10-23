
using System;
using XOutput.Common.Devices;
using XOutput.Common.Input;

namespace XOutput.Mapping.Input
{
    public class InputDeviceSourceWithValue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SourceTypes Type { get; set; }
        public double Value { get; set; }

        public static InputDeviceSourceWithValue Create(InputDeviceSource source) {
            SourceTypes type = (SourceTypes) Enum.Parse(typeof(SourceTypes), source.Type);
            return new InputDeviceSourceWithValue {
                Id = source.Id,
                Name = source.Name,
                Type = type,
                Value = 0,
            };
        }
    }
}
