using System;
using System.Collections.Generic;
using XOutput.DependencyInjection;
using XOutput.Serialization;
using XOutput.Websocket.Common;
using XOutput.Websocket.Ds4;
using XOutput.Websocket.Input;
using XOutput.Websocket.Xbox;

namespace XOutput
{
    public static class ServerConfiguration
    {
        [ResolverMethod]
        public static MessageReader MessageReader()
        {
            Dictionary<string, Type> deserializationMapping = new Dictionary<string, Type>
            {
                { DebugRequest.MessageType,  typeof(DebugRequest) },
                { PingRequest.MessageType,  typeof(PingRequest) },
                { PongResponse.MessageType,  typeof(PongResponse) },
                { XboxInputRequest.MessageType,  typeof(XboxInputRequest) },
                { Ds4InputRequest.MessageType,  typeof(Ds4InputRequest) },
                { InputDeviceDetailsRequest.MessageType,  typeof(InputDeviceDetailsRequest) },
                { InputDeviceInputRequest.MessageType,  typeof(InputDeviceInputRequest) },
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
