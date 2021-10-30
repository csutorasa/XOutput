namespace XOutput.Websocket.Common
{
    public class PongResponse : MessageBase
    {
        public const string MessageType = "Pong";

        public long Timestamp { get; set; }
        public PongResponse()
        {
            Type = MessageType;
        }
    }
}
