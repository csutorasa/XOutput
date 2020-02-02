namespace XOutput.Api.Message.Xbox
{
    public class XboxFeedbackMessage : MessageBase
    {
        public const string MessageType = "XboxFeedback";

        public XboxFeedbackMessage()
        {
            Type = MessageType;
        }

        public double Small { get; set; }
        public double Large { get; set; }
        public int LedNumber { get; set; }
    }
}
