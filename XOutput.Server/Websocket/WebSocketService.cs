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

namespace XOutput.Server.Websocket
{
    public class WebSocketService
    {

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly List<IMessageHandler> restHandlers;
        private readonly MessageReader messageReader;
        private readonly MessageWriter messageWriter;

        [ResolverMethod]
        public WebSocketService(ApplicationContext applicationContext, MessageReader messageReader, MessageWriter messageWriter)
        {
            restHandlers = applicationContext.ResolveAll<IMessageHandler>();
            this.messageReader = messageReader;
            this.messageWriter = messageWriter;
        }

        public bool Handle(HttpListenerContext httpContext, CancellationToken cancellationToken)
        {
            if (!httpContext.Request.IsWebSocketRequest)
            {
                return false;
            }
            Task.Run(() => HandleWebSocketAsync(httpContext, cancellationToken));
            return true;
        }

        private async Task HandleWebSocketAsync(HttpListenerContext httpContext, CancellationToken cancellationToken)
        {
            try
            {
                var websocketContext = await httpContext.AcceptWebSocketAsync(null).ConfigureAwait(false);
                var ws = websocketContext.WebSocket;
                if (ws == null)
                {
                    return;
                }
                using (ws)
                // using (var outputDevice = new WebXOutputDevice(xOutputManager))
                {
                    var websocketSessionContext = new WebsocketSessionContext((message) => WriteStringAsync(ws, cancellationToken, messageWriter.WriteMessage(message)));
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
                            List<IMessageHandler> acceptedHandlers = restHandlers.Where(h => h.HandledType == message.GetType()).ToList();
                            if (acceptedHandlers.Count == 0)
                            {
                                logger.Error("No handlers found for {0}", message.Type);
                            }
                            else if (acceptedHandlers.Count == 1)
                            {
                                acceptedHandlers[0].HandleMessage(message, websocketSessionContext);
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

        private Task WriteStringAsync(WebSocket ws, CancellationToken cancellationToken, string message)
        {
            var data = Encoding.UTF8.GetBytes(message);
            ArraySegment<byte> buffer = new ArraySegment<byte>(data);
            return ws.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
        }
    }
}
