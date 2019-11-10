using XOutput.Devices.Mapper;

namespace XOutput.Devices.XInput
{
    /// <summary>
    /// Direct input source.
    /// </summary>
    public class XOutputSource : InputSource
    {
        public XInputTypes XInputType => inputType;

        private readonly XInputTypes inputType;


        public XOutputSource(string name, XInputTypes type) : base(null, name, type.GetInputSourceType(), 0)
        {
            inputType = type;
        }

        internal bool Refresh(InputMapper mapper)
        {
            var mappingCollection = mapper.GetMapping(inputType);
            if (mappingCollection != null)
            {
                double newValue = mappingCollection.GetValue(inputType);
                return RefreshValue(newValue);
            }
            return false;
        }
    }
}
