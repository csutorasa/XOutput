namespace XOutput.Websocket.Common
{
    public class PingRequest : MessageBase
    {
        public const string MessageType = "Ping";

        public long Timestamp { get; set; }
        public PingRequest()
        {
            Type = MessageType;
        }
    }
}
