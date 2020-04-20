using System.Collections.Generic;

namespace XOutput.Api.Message.Input
{
    public class InputValuesMessage : MessageBase
    {
        public const string MessageType = "InputValues";

        public InputValuesMessage()
        {
            Type = MessageType;
        }

        public Dictionary<int, double> Values { get; set; }
    }
}
