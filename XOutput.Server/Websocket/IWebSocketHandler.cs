using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using XOutput.Api.Message;

namespace XOutput.Server.Websocket
{
    interface IWebSocketHandler
    {
        bool CanHandle(HttpListenerContext context);
        List<IMessageHandler> CreateHandlers(HttpListenerContext context, Func<MessageBase, Task> sendFunction);
    }
}
