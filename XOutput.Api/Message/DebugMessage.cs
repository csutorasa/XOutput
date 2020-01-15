namespace XOutput.Api.Message
{
    public class DebugMessage : MessageBase
    {
        public const string MessageType = "Debug";

        public string Data { get; set; }
    }
}
