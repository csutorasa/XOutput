using System.Collections.Generic;

namespace XOutput.Api.Message
{
    public class InputDataMessage : MessageBase
    {
        public const string MessageType = "InputData";

        public List<InputData> Data { get; set; }
    }

    public class InputData
    {
        public string InputType { get; set; }
        public double Value { get; set; }
    }
}
