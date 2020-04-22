using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using XOutput.Api.Message;
using XOutput.Api.Message.Ds4;
using XOutput.Api.Message.Xbox;

namespace XOutput.Api.Serialization
{
    class JsonMessageConverter : JsonConverter
    {
        private static Dictionary<string, Func<MessageBase>> messageTypeMapping = new Dictionary<string, Func<MessageBase>>();

        static JsonMessageConverter()
        {
            messageTypeMapping.Add(InputDataMessage.MessageType, () => new InputDataMessage());
            messageTypeMapping.Add(DebugMessage.MessageType, () => new DebugMessage());
            messageTypeMapping.Add(XboxInputMessage.MessageType, () => new XboxInputMessage());
            messageTypeMapping.Add(Ds4InputMessage.MessageType, () => new Ds4InputMessage());
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(MessageBase).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            var messageType = (string)jObject["Type"];
            MessageBase target = CreateMessage(messageType);
            serializer.Populate(jObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private MessageBase CreateMessage(string type)
        {
            if (messageTypeMapping.ContainsKey(type))
            {
                return messageTypeMapping[type]();
            }
            return new MessageBase { Type = type };
        }
    }
}
