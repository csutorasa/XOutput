namespace XOutput.Api.Message.Ds4
{
    public class Ds4InputMessage : MessageBase
    {
        public const string MessageType = "Ds4Input";

        public Ds4InputMessage()
        {
            Type = MessageType;
        }

        public bool? Circle { get; set; }
        public bool? Cross { get; set; }
        public bool? Triangle { get; set; }
        public bool? Square { get; set; }
        public bool? L1 { get; set; }
        public bool? L3 { get; set; }
        public bool? R1 { get; set; }
        public bool? R3 { get; set; }
        public bool? Options { get; set; }
        public bool? Share { get; set; }
        public bool? Ps { get; set; }
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
