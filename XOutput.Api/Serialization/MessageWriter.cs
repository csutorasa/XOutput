using Newtonsoft.Json;
using System.IO;
using System.Text;
using XOutput.Api.Message;

namespace XOutput.Api.Serialization
{
    public class MessageWriter
    {
        private readonly JsonSerializer jsonSerializer = new JsonSerializer();

        public string GetString(MessageBase message)
        {
            return JsonConvert.SerializeObject(message);
        }

        public byte[] GetBytes(MessageBase message, Encoding encoding)
        {
            return encoding.GetBytes(GetString(message));
        }

        public void Write(MessageBase message, StreamWriter output)
        {
            jsonSerializer.Serialize(output, message);
            output.Flush();
        }
    }
}
