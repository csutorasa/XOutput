using System.Collections.Generic;

namespace XOutput.Websocket.Input
{
    public class InputDeviceInputResponse : MessageBase
    {
        public const string MessageType = "InputDeviceInputFeedback";
        public List<InputDeviceSourceValue> Sources { get; set; }
        public List<InputDeviceTargetValue> Targets { get; set; }
        public InputDeviceInputResponse()
        {
            Type = MessageType;
        }
    }
}
