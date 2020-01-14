using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Api.Message
{
    public class DebugMessage : MessageBase
    {
        public const string MessageType = "Debug";

        public string Data { get; set; }
    }
}
