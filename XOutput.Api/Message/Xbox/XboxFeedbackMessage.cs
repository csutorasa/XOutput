namespace XOutput.Api.Message.Xbox
{
    public class XboxFeedbackMessage : MessageBase
    {
        public const string MessageType = "XboxFeedback";

        public XboxFeedbackMessage()
        {
            Type = MessageType;
        }

        public double SmallForceFeedback { get; set; }
        public double BigForceFeedback { get; set; }
        public int LedNumber { get; set; }
    }
}
