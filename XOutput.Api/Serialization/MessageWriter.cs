using Newtonsoft.Json;
using System.IO;
using XOutput.Api.Message;

namespace XOutput.Api.Serialization
{
    public class MessageWriter
    {
        public string WriteMessage(MessageBase message)
        {
            return JsonConvert.SerializeObject(message);
        }

        public void WriteMessage(MessageBase message, StreamWriter output)
        {
            output.Write(WriteMessage(message));
            output.Flush();
        }
    }
}
