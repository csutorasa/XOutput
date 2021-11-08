namespace XOutput.Websocket.Xbox
{
    public class XboxInputRequest : MessageBase
    {
        public const string MessageType = "XboxInput";

        public XboxInputRequest()
        {
            Type = MessageType;
        }

        public bool? A { get; set; }
        public bool? B { get; set; }
        public bool? X { get; set; }
        public bool? Y { get; set; }
        public bool? L1 { get; set; }
        public bool? L3 { get; set; }
        public bool? R1 { get; set; }
        public bool? R3 { get; set; }
        public bool? Start { get; set; }
        public bool? Back { get; set; }
        public bool? Home { get; set; }
        public bool? Up { get; set; }
        public bool? Down { get; set; }
        public bool? Left { get; set; }
        public bool? Right { get; set; }
        public double? LX { get; set; }
        public double? LY { get; set; }
        public double? RX { get; set; }
        public double? RY { get; set; }
        public double? L2 { get; set; }
        public double? R2 { get; set; }
    }
}
