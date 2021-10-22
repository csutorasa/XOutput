namespace XOutput.Api.Message.Ds4
{
    public class Ds4FeedbackMessage : MessageBase
    {
        public const string MessageType = "Ds4Feedback";

        public Ds4FeedbackMessage()
        {
            Type = MessageType;
        }

        public double SmallForceFeedback { get; set; }
        public double BigForceFeedback { get; set; }
    }
}
