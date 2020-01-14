using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Api.Message;

namespace XOutput.Server.Websocket
{
    class WebsocketSessionContext
    {
        private readonly Func<MessageBase, Task> sendFunction;

        public WebsocketSessionContext(Func<MessageBase, Task> sendFunction)
        {
            this.sendFunction = sendFunction;
        }

        public Task WriteMessageAsync(MessageBase message)
        {
            return sendFunction(message);
        }
    }
}
