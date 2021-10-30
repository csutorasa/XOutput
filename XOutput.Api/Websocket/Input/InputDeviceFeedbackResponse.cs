
using System.Collections.Generic;

namespace XOutput.Websocket.Input
{
    public class InputDeviceFeedbackResponse : MessageBase
    {
        public const string MessageType = "InputDeviceFeedback";
        public List<InputDeviceTargetValue> Targets { get; set; }
        public InputDeviceFeedbackResponse()
        {
            Type = MessageType;
        }
    }
}
