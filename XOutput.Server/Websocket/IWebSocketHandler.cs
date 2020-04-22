using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net;

namespace XOutput.Server.Websocket
{
    public interface IWebSocketHandler
    {
        bool CanHandle(HttpContext context);
        List<IMessageHandler> CreateHandlers(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction);
        void Close(IEnumerable<IMessageHandler> handlers);
    }
}
