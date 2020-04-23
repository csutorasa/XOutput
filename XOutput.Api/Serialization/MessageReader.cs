using Newtonsoft.Json;
using System.IO;
using System.Text;
using XOutput.Api.Message;

namespace XOutput.Api.Serialization
{
    public class MessageReader
    {
        private readonly JsonMessageConverter converter = new JsonMessageConverter();
        private readonly JsonSerializer jsonSerializer = new JsonSerializer();

        public MessageReader()
        {
            jsonSerializer.Converters.Add(converter);
        }

        public MessageBase ReadString(string input)
        {
            return JsonConvert.DeserializeObject<MessageBase>(input, converter);
        }

        public MessageBase ReadMessage(byte[] input, Encoding encoding)
        {
            return ReadString(encoding.GetString(input));
        }

        public MessageBase Read(StreamReader input)
        {
            return (MessageBase)jsonSerializer.Deserialize(input, typeof(MessageBase));
        }
    }
}
