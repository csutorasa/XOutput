
using XOutput.Api.Message;

namespace XOutput.Message.Mappable
{
    public class MappableDeviceFeedbackMessage : MessageBase
    {
        public const string MessageType = "MappableDeviceFeedback";
        public double SmallForceFeedback { get; set; }
        public double BigForceFeedback { get; set; }
    }
}
