
using System.Collections.Generic;

namespace XOutput.Websocket.Input
{
    public class InputDeviceInputRequest : MessageBase
    {
        public const string MessageType = "InputDeviceInput";
        public List<InputDeviceSourceValue> Inputs { get; set; }
    }
}
