namespace XOutput.Websocket.Ds4
{
    public class Ds4FeedbackResponse : MessageBase
    {
        public const string MessageType = "Ds4Feedback";

        public Ds4FeedbackResponse()
        {
            Type = MessageType;
        }

        public double SmallForceFeedback { get; set; }
        public double BigForceFeedback { get; set; }
    }
}
