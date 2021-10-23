using System;
using System.Collections.Generic;
using XOutput.Message.Mappable;
using XOutput.Websocket.Xbox;
using XOutput.Websocket.Common;
using XOutput.Websocket.Ds4;
using XOutput.Serialization;
using XOutput.DependencyInjection;

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
                { MappableDeviceFeedbackResponse.MessageType,  typeof(MappableDeviceFeedbackResponse) },
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
