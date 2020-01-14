using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }
    }
}
