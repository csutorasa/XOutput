
using XOutput.Websocket;

namespace XOutput.Message.Mappable
{
    public class MappableDeviceOutputResponse : MessageBase
    {
        public const string MessageType = "MappableDeviceOutput";
        public double SmallForceFeedback { get; set; }
        public double BigForceFeedback { get; set; }
    }
}
