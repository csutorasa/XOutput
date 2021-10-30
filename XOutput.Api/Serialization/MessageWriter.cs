using System.IO;
using System.Text.Json;
using XOutput.Websocket;

namespace XOutput.Serialization
{
    public class MessageWriter
    {
        private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public string GetString(MessageBase message)
        {
            return JsonSerializer.Serialize(message, message.GetType(), serializerOptions);
        }

        public void Write(MessageBase message, Stream output)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(message, serializerOptions);
            output.Write(bytes);
            output.Flush();
        }
    }
}
