using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Api.Serialization;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Threading;
using XOutput.Core.WebSocket;

namespace XOutput.Server.Websocket
{
    public class WebSocketService
    {

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly List<IWebSocketHandler> webSocketHandlers;
        private readonly MessageReader messageReader;
        private readonly MessageWriter messageWriter;
        private readonly WebSocketHelper webSocketHelper;

        [ResolverMethod]
        public WebSocketService(ApplicationContext applicationContext, MessageReader messageReader, MessageWriter messageWriter, WebSocketHelper webSocketHelper)
        {
            webSocketHandlers = applicationContext.ResolveAll<IWebSocketHandler>();
            this.messageReader = messageReader;
            this.messageWriter = messageWriter;
            this.webSocketHelper = webSocketHelper;
        }

        public bool Handle(HttpListenerContext httpContext, CancellationToken cancellationToken)
        {
            if (!httpContext.Request.IsWebSocketRequest)
            {
                return false;
            }
            ThreadCreator.Create("Websocket listener", (token) => HandleWebSocketAsync(httpContext, cancellationToken)).Start();
            return true;
        }

        private async Task HandleWebSocketAsync(HttpListenerContext httpContext, CancellationToken cancellationToken)
        {
            try
            {
                List<IWebSocketHandler> acceptedHandlers = webSocketHandlers.Where(h => h.CanHandle(httpContext)).ToList();
                if (acceptedHandlers.Count == 0)
                {
                    httpContext.Response.StatusCode = 404;
                    httpContext.Response.Close();
                    return;
                }
                else if (acceptedHandlers.Count > 1)
                {
                    logger.Error("Multiple handlers found for {0}", httpContext.Request.Url);
                    httpContext.Response.StatusCode = 500;
                    httpContext.Response.Close();
                    return;
                }
                var acceptedHandler = acceptedHandlers[0];
                var websocketContext = await httpContext.AcceptWebSocketAsync(null).ConfigureAwait(false);
                var ws = websocketContext.WebSocket;
                if (ws == null)
                {
                    return;
                }
                using (ws)
                {
                    await HandleWebSocketContextAsync(ws, httpContext, acceptedHandler, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while handling websocket");
            }
        }

        private Task WriteStringAsync(WebSocket ws, string message, CancellationToken cancellationToken)
        {
            var data = Encoding.UTF8.GetBytes(message);
            ArraySegment<byte> buffer = new ArraySegment<byte>(data);
            return ws.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
        }

        private async Task HandleWebSocketContextAsync(WebSocket ws, HttpListenerContext httpContext, IWebSocketHandler handler, CancellationToken cancellationToken)
        {
            var messageHandlers = handler.CreateHandlers(httpContext, (message) => WriteStringAsync(ws, messageWriter.GetString(message), cancellationToken));

            while (ws.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                string requestMessage = await webSocketHelper.ReadStringAsync(ws, Encoding.UTF8, cancellationToken);
                if (requestMessage == null)
                {
                    continue;
                }
                ProcessMessage(requestMessage, messageHandlers);
            }
            if (ws.State != WebSocketState.Closed)
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
            }
            messageHandlers.ForEach(h => h.Close());
        }

        private void ProcessMessage(string requestMessage, List<IMessageHandler> messageHandlers)
        {
            try
            {
                var message = messageReader.ReadString(requestMessage);
                var acceptedMessageHandlers = messageHandlers.Where(h => h.CanHandle(message)).ToList();
                if (acceptedMessageHandlers.Count == 0)
                {
                    logger.Error("No handlers found for {0}", message.Type);
                }
                else if (acceptedMessageHandlers.Count == 1)
                {
                    acceptedMessageHandlers[0].Handle(message);
                }
                else
                {
                    logger.Error("Multiple handlers found for {0}", message.Type);
                }
            }
            catch (Exception e)
            {
                logger.Warn(e, "Error while handling websocket message: " + requestMessage);
            }
        }
    }
}
