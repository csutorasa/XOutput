
using XOutput.Websocket;

namespace XOutput.Message.Mappable
{
    public class MappableDeviceFeedbackResponse : MessageBase
    {
        public const string MessageType = "MappableDeviceFeedback";
        public double SmallForceFeedback { get; set; }
        public double BigForceFeedback { get; set; }
    }
}
