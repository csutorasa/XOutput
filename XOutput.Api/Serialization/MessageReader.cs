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
    public class MessageReader
    {
        private JsonMessageConverter converter = new JsonMessageConverter();

        public MessageBase ReadMessage(StreamReader input)
        {
            return ReadMessage(input.ReadToEnd());
        }

        public MessageBase ReadMessage(string input)
        {
            return JsonConvert.DeserializeObject<MessageBase>(input);
        }
    }
}
