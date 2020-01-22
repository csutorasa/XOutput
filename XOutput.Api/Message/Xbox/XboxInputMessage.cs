namespace XOutput.Api.Message.Xbox
{
    public class XboxInputMessage : MessageBase
    {
        public const string MessageType = "XboxInput";

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
        public bool? UP { get; set; }
        public bool? DOWN { get; set; }
        public bool? LEFT { get; set; }
        public bool? RIGHT { get; set; }
        public double? LX { get; set; }
        public double? LY { get; set; }
        public double? RX { get; set; }
        public double? RY { get; set; }
        public double? L2 { get; set; }
        public double? R2 { get; set; }
    }
}
