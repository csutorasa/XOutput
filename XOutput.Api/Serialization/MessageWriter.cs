using System.IO;
using System.Text.Json;
using XOutput.Api.Message;

namespace XOutput.Api.Serialization
{
    public class MessageWriter
    {
        private readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public string GetString<T>(T message) where T: MessageBase
        {
            return JsonSerializer.Serialize(message, serializerOptions);
        }

        public void Write(MessageBase message, Stream output)
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(message, serializerOptions);
            output.Write(bytes);
            output.Flush();
        }
    }
}
