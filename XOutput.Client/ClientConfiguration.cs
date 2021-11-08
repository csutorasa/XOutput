using System;
using System.Collections.Generic;
using XOutput.DependencyInjection;
using XOutput.Serialization;
using XOutput.Websocket.Common;
using XOutput.Websocket.Ds4;
using XOutput.Websocket.Emulation;
using XOutput.Websocket.Input;
using XOutput.Websocket.Xbox;

namespace XOutput.Client
{
    public static class ClientConfiguration
    {
        [ResolverMethod]
        public static MessageReader MessageReader()
        {
            Dictionary<string, Type> deserializationMapping = new Dictionary<string, Type>
            {
                { DebugRequest.MessageType,  typeof(DebugRequest) },
                { PingRequest.MessageType,  typeof(PingRequest) },
                { PongResponse.MessageType,  typeof(PongResponse) },
                { XboxFeedbackResponse.MessageType,  typeof(XboxFeedbackResponse) },
                { Ds4FeedbackResponse.MessageType,  typeof(Ds4FeedbackResponse) },
                { InputDeviceFeedbackResponse.MessageType,  typeof(InputDeviceFeedbackResponse) },
                { InputDeviceInputResponse.MessageType,  typeof(InputDeviceInputResponse) },
                { ControllerInputResponse.MessageType,  typeof(ControllerInputResponse) },
            };
            return new MessageReader(deserializationMapping);
        }

        [ResolverMethod]
        public static MessageWriter MessageWriter()
        {
            return new MessageWriter();
        }
    }
}
