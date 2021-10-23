namespace XOutput.Websocket.Input
{
    public class InputDeviceInputResponse : MessageBase
    {
        public const string MessageType = "InputDeviceInputFeedback";
        public double SmallForceFeedback { get; set; }
        public double BigForceFeedback { get; set; }
    }
}
