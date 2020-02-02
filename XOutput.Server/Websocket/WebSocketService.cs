using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Api.Serialization;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Threading;

namespace XOutput.Server.Websocket
{
    public class WebSocketService
    {

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly List<IWebSocketHandler> webSocketHandlers;
        private readonly MessageReader messageReader;
        private readonly MessageWriter messageWriter;

        [ResolverMethod]
        public WebSocketService(ApplicationContext applicationContext, MessageReader messageReader, MessageWriter messageWriter)
        {
            webSocketHandlers = applicationContext.ResolveAll<IWebSocketHandler>();
            this.messageReader = messageReader;
            this.messageWriter = messageWriter;
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
                    var messageHandlers = acceptedHandler.CreateHandlers(httpContext, (message) => WriteStringAsync(ws, messageWriter.WriteMessage(message), cancellationToken));

                    while (ws.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                    {
                        string requestMessage = await ReadStringAsync(ws, cancellationToken).ConfigureAwait(false);
                        if (requestMessage == null)
                        {
                            continue;
                        }
                        try
                        {
                            var message = messageReader.ReadMessage(requestMessage);
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
                            continue;
                        }
                    }
                    if (ws.State != WebSocketState.Closed)
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
                    }
                    messageHandlers.ForEach(h => h.Close());
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error while handling websocket");
            }
        }

        private async Task<string> ReadStringAsync(WebSocket ws, CancellationToken cancellationToken)
        {
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[8192]);
                do
                {
                    result = await ws.ReceiveAsync(buffer, cancellationToken);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                if (result.CloseStatus != null)
                {
                    return null;
                }

                ms.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private Task WriteStringAsync(WebSocket ws, string message, CancellationToken cancellationToken)
        {
            var data = Encoding.UTF8.GetBytes(message);
            ArraySegment<byte> buffer = new ArraySegment<byte>(data);
            return ws.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
        }
    }
}
