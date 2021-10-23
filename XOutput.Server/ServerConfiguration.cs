using System;
using System.Collections.Generic;
using XOutput.Message.Mappable;
using XOutput.Serialization;
using XOutput.Websocket.Common;
using XOutput.Websocket.Ds4;
using XOutput.Websocket.Xbox;

namespace XOutput
{
    public static class ServerConfiguration
    {
        public static MessageReader MessageReader()
        {
            Dictionary<string, Type> deserializationMapping = new Dictionary<string, Type>
            {
                { DebugRequest.MessageType,  typeof(DebugRequest) },
                { PingRequest.MessageType,  typeof(PingRequest) },
                { PongResponse.MessageType,  typeof(PongResponse) },
                { XboxInputRequest.MessageType,  typeof(XboxInputRequest) },
                { Ds4InputRequest.MessageType,  typeof(Ds4InputRequest) },
                { MappableDeviceDetailsRequest.MessageType,  typeof(MappableDeviceDetailsRequest) },
                { MappableDeviceInputRequest.MessageType,  typeof(MappableDeviceInputRequest) },
            };
            return new MessageReader(deserializationMapping);
        }

        public static MessageWriter MessageWriter()
        {
            return new MessageWriter();
        }
    }
}
