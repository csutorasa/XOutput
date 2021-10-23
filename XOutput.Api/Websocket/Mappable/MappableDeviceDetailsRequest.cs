
using System.Collections.Generic;
using XOutput.Websocket;

namespace XOutput.Message.Mappable
{
    public class MappableDeviceDetailsRequest : MessageBase
    {
        public const string MessageType = "MappableDeviceDetails";
        public string Id { get; set; }
        public string Name { get; set; }
        public string HardwareId { get; set; }
        public List<MappableDeviceSource> Sources { get; set; }
        public string InputMethod { get; set; }
    }

    public class MappableDeviceSource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
