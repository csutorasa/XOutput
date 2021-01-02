using System;
using System.Collections.Generic;
using XOutput.Api.Message;
using XOutput.Api.Message.Ds4;
using XOutput.Api.Message.Xbox;
using XOutput.Api.Serialization;
using XOutput.Message.Mappable;

namespace XOutput.Server
{
    public static class ServerConfiguration
    {
        public static MessageReader MessageReader()
        {
            Dictionary<string, Type> deserializationMapping = new Dictionary<string, Type>
            {
                { InputDataMessage.MessageType, typeof(InputDataMessage) },
                { DebugMessage.MessageType,  typeof(DebugMessage) },
                { XboxInputMessage.MessageType,  typeof(XboxInputMessage) },
                { Ds4InputMessage.MessageType,  typeof(Ds4InputMessage) },
                { MappableDeviceDetailsMessage.MessageType,  typeof(MappableDeviceDetailsMessage) },
                { MappableDeviceInputMessage.MessageType,  typeof(MappableDeviceInputMessage) },
            };
            return new MessageReader(deserializationMapping);
        }

        public static MessageWriter MessageWriter()
        {
            return new MessageWriter();
        }
    }
}
