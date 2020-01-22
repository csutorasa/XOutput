using Newtonsoft.Json;
using System.IO;
using XOutput.Api.Message;

namespace XOutput.Api.Serialization
{
    public class MessageReader
    {
        private readonly JsonMessageConverter converter = new JsonMessageConverter();

        public MessageBase ReadMessage(StreamReader input)
        {
            return ReadMessage(input.ReadToEnd());
        }

        public MessageBase ReadMessage(string input)
        {
            return JsonConvert.DeserializeObject<MessageBase>(input, converter);
        }
    }
}
