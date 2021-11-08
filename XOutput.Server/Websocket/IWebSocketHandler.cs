using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace XOutput.Websocket
{
    public interface IWebSocketHandler
    {
        public const string WebsocketBasePath = "/websocket";
        bool CanHandle(HttpContext context);
        IMessageHandler CreateHandler(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction);
    }
}
