namespace XOutput.Websocket.Xbox
{
    public class XboxFeedbackResponse : MessageBase
    {
        public const string MessageType = "XboxFeedback";

        public XboxFeedbackResponse()
        {
            Type = MessageType;
        }

        public double SmallForceFeedback { get; set; }
        public double BigForceFeedback { get; set; }
        public int LedNumber { get; set; }
    }
}
