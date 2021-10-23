namespace XOutput.Websocket.Input
{
    public class InputDeviceOutputResponse : MessageBase
    {
        public const string MessageType = "InputDeviceOutputFeedback";
        public double SmallForceFeedback { get; set; }
        public double BigForceFeedback { get; set; }
    }
}
