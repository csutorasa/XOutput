using System;
using System.Collections.Generic;
using XOutput.Api.Message;
using XOutput.Api.Message.Ds4;
using XOutput.Api.Message.Xbox;
using XOutput.Api.Serialization;
using XOutput.Message.Mappable;

namespace XOutput.Client
{
    public static class ClientConfiguration
    {
        public static MessageReader MessageReader()
        {
            Dictionary<string, Type> deserializationMapping = new Dictionary<string, Type>
            {
                { InputDataMessage.MessageType, typeof(InputDataMessage) },
                { DebugMessage.MessageType,  typeof(DebugMessage) },
                { XboxFeedbackMessage.MessageType,  typeof(XboxFeedbackMessage) },
                { Ds4FeedbackMessage.MessageType,  typeof(Ds4FeedbackMessage) },
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
