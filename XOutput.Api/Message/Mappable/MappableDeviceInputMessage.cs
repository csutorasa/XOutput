
using System.Collections.Generic;
using XOutput.Api.Message;

namespace XOutput.Message.Mappable
{
    public class MappableDeviceInputMessage : MessageBase
    {
        public const string MessageType = "MappableDeviceInput";
        public List<MappableDeviceSourceValue> Inputs { get; set; }
    }

    public class MappableDeviceSourceValue
    {
        public int Id { get; set; }
        public double Value { get; set; }
    }
}
