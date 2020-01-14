using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Api.Message
{
    public class InputDataMessage : MessageBase
    {
        public const string MessageType = "InputData";

        public List<InputData> Data { get; set; }
    }

    public struct InputData
    {
        public string InputType { get; set; }
        public double Value { get; set; }
    }
}
