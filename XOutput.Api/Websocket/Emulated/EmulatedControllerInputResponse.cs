using System.Collections.Generic;

namespace XOutput.Websocket.Emulated
{
    public class EmulatedControllerInputResponse : MessageBase
    {
        public const string MessageType = "EmulatedControllerInputFeedback";
        public List<EmulatedControllerSourceValue> Sources { get; set; }
        public List<EmulatedControllerTargetValue> Targets { get; set; }
        public EmulatedControllerInputResponse()
        {
            Type = MessageType;
        }
    }
}
