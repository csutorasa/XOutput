using Microsoft.AspNetCore.Http;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.DependencyInjection;
using XOutput.Serialization;

namespace XOutput.Websocket
{
    public class WebSocketService
    {

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly List<IWebSocketHandler> webSocketHandlers;
        private readonly MessageReader messageReader;
        private readonly MessageWriter messageWriter;
        private readonly WebSocketHelper webSocketHelper;

        [ResolverMethod]
        public WebSocketService(List<IWebSocketHandler> webSocketHandlers, WebSocketHelper webSocketHelper, MessageReader messageReader)
        {
            this.webSocketHandlers = webSocketHandlers;
            this.messageReader = messageReader;
            messageWriter = new MessageWriter();
            this.webSocketHelper = webSocketHelper;
        }

        public async Task HandleWebSocketAsync(HttpContext httpContext, CancellationToken cancellationToken)
        {
            try
            {
                List<IWebSocketHandler> acceptedHandlers = webSocketHandlers.Where(h => h.CanHandle(httpContext)).ToList();
                if (acceptedHandlers.Count == 0)
                {
                    logger.Error("No handlers found for {0}", httpContext.Request.Path);
                    httpContext.Response.StatusCode = 404;
                    return;
                }
                if (acceptedHandlers.Count > 1)
                {
                    logger.Error("Multiple handlers found for {0}", httpContext.Request.Path);
                    httpContext.Response.StatusCode = 500;
                    return;
                }
                var acceptedHandler = acceptedHandlers[0];
                var ws = await httpContext.WebSockets.AcceptWebSocketAsync(null).ConfigureAwait(false);
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
            if (ws.State == WebSocketState.Open)
            {
                return ws.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
            }
            return ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
        }

        private async Task HandleWebSocketContextAsync(WebSocket ws, HttpContext httpContext, IWebSocketHandler handler, CancellationToken cancellationToken)
        {
            CloseFunction closeFunction = () => ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
            List<IMessageHandler> messageHandlers;
            try
            {
                messageHandlers = handler.CreateHandlers(httpContext, closeFunction, (message) => WriteStringAsync(ws, messageWriter.GetString(message), cancellationToken));
            }
            catch (Exception e)
            {
                logger.Error(e, "Error occured while creating handlers for {0}", httpContext.Request.Path);
                httpContext.Response.StatusCode = 500;
                return;
            }

            while (ws.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                string requestMessage = await webSocketHelper.ReadStringAsync(ws, Encoding.UTF8, cancellationToken);
                if (requestMessage == null)
                {
                    continue;
                }
                ProcessMessage(requestMessage, messageHandlers);
            }
            if (ws.State == WebSocketState.Open)
            {
                await closeFunction();
            }
            handler.Close(messageHandlers);
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
