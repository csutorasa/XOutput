namespace XOutput.Api.Message.Ds4
{
    public class Ds4FeedbackMessage : MessageBase
    {
        public const string MessageType = "Ds4Feedback";

        public Ds4FeedbackMessage()
        {
            Type = MessageType;
        }

        public double Small { get; set; }
        public double Large { get; set; }
    }
}
