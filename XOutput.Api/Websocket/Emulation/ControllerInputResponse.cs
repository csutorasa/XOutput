using System.Collections.Generic;

namespace XOutput.Websocket.Emulation
{
    public class ControllerInputResponse : MessageBase
    {
        public const string MessageType = "ControllerInputFeedback";
        public List<ControllerSourceValue> Sources { get; set; }
        public List<ControllerTargetValue> Targets { get; set; }
        public ControllerInputResponse()
        {
            Type = MessageType;
        }
    }
}
