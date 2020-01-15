namespace XOutput.Api.Message
{
    public class ForceFeedbackMessage : MessageBase
    {
        public const string MessageType = "ForceFeedback";

        public ForceFeedbackMessage()
        {
            Type = MessageType;
        }

        public double Small { get; set; }
        public double Large { get; set; }
    }
}
