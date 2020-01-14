using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Api.Message;
using XOutput.Api.Serialization;
using XOutput.Devices.XInput;
using XOutput.Logging;
using XOutput.Tools;

namespace XOutput.Server
{
    public class WebSocketService
    {
        private static ILogger logger = LoggerFactory.GetLogger(typeof(WebSocketService));

        private readonly XOutputManager xOutputManager;

        [ResolverMethod]
        public WebSocketService(XOutputManager xOutputManager)
        {
            this.xOutputManager = xOutputManager;
        }

        public bool Handle(HttpListenerContext httpContext, CancellationToken cancellationToken)
        {
            if (!httpContext.Request.IsWebSocketRequest)
            {
                return false;
            }
            if (!xOutputManager.HasDevice)
            { 
                httpContext.Response.StatusCode = 500;
                httpContext.Response.Close();
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
                using (var outputDevice = new WebXOutputDevice(xOutputManager))
                {
                    while (ws.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                    {
                        string requestMessage = await ReadString(ws, cancellationToken).ConfigureAwait(false);
                        if(requestMessage == null)
                        {
                            continue;
                        }
                        try
                        {
                            var message = new MessageReader().ReadMessage(requestMessage);
                            ProcessMessage(outputDevice, message);
                        }
                        catch (Exception)
                        { 
                            logger.Warning("Invalid websocket message: " + requestMessage);
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
                logger.Error("Error while handling websocket", ex);
            }
        }

        private async Task<string> ReadString(WebSocket ws, CancellationToken cancellationToken)
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

        private void ProcessMessage(WebXOutputDevice device, MessageBase message)
        {
            string messageType = message.Type;
            if (messageType == InputDataMessage.MessageType)
            {
                var inputs = (message as InputDataMessage).Data;
                foreach (var input in inputs)
                {
                    XInputTypes type;
                    if (!Enum.TryParse(input.InputType, out type))
                    {
                        logger.Error("Invalid input message: " + input);
                        continue;
                    }
                    device.Sources.OfType<WebXOutputSource>().First(s => s.XInputType == type).SetValue(input.Value);
                }
            }
            else if (messageType == DebugMessage.MessageType)
            {
                logger.Info("Message from client: " + (message as DebugMessage).Data);
            }
            else
            {
                logger.Warning("Unknown messageType: " + messageType);
            }
        }
    }
}
