using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace XOutput.Websocket
{
    public interface ICommonWebSocketHandler
    {
        bool CanHandle(HttpContext context);
        IWebSocketHandler CreateHandlers(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction);
        void Close();
    }
}
