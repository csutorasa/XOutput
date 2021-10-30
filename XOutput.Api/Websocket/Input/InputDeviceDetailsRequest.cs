
using System.Collections.Generic;
using XOutput.Common.Input;

namespace XOutput.Websocket.Input
{
    public class InputDeviceDetailsRequest : MessageBase
    {
        public const string MessageType = "InputDeviceDetails";
        public string Id { get; set; }
        public string Name { get; set; }
        public string HardwareId { get; set; }
        public List<InputDeviceSource> Sources { get; set; }
        public List<InputDeviceTarget> Targets { get; set; }
        public string InputApi { get; set; }
        public InputDeviceDetailsRequest()
        {
            Type = MessageType;
        }
    }
}
