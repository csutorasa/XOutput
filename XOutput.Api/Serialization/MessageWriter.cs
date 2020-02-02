using Newtonsoft.Json;
using System.IO;
using System.Text;
using XOutput.Api.Message;

namespace XOutput.Api.Serialization
{
    public class MessageWriter
    {

        public string WriteMessage(MessageBase message)
        {
            return JsonConvert.SerializeObject(message);
        }

        public byte[] WriteMessage(MessageBase message, Encoding encoding)
        {
            return encoding.GetBytes(WriteMessage(message));
        }

        public void WriteMessage(MessageBase message, StreamWriter output)
        {
            output.Write(WriteMessage(message));
            output.Flush();
        }
    }
}
