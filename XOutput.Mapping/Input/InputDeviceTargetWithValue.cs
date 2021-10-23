using XOutput.Common.Input;

namespace XOutput.Mapping.Input
{
    public class InputDeviceTargetWithValue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }

        public static InputDeviceTargetWithValue Create(InputDeviceTarget source) {
            return new InputDeviceTargetWithValue {
                Id = source.Id,
                Name = source.Name,
                Value = 0,
            };
        }
    }
}
