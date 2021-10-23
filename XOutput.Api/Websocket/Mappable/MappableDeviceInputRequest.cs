
using System.Collections.Generic;
using XOutput.Websocket;

namespace XOutput.Message.Mappable
{
    public class MappableDeviceInputRequest : MessageBase
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
