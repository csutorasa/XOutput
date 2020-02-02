using System.Collections.Generic;
using System.Net;

namespace XOutput.Server.Websocket
{
    interface IWebSocketHandler
    {
        bool CanHandle(HttpListenerContext context);
        List<IMessageHandler> CreateHandlers(HttpListenerContext context, SenderFunction sendFunction);
    }
}
