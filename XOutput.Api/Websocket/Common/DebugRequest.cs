namespace XOutput.Websocket.Common
{
    public class DebugRequest : MessageBase
    {
        public const string MessageType = "Debug";

        public string Data { get; set; }
        public DebugRequest()
        {
            Type = MessageType;
        }
    }
}
