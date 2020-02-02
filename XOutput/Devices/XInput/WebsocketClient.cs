using NLog;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Api.Devices;
using XOutput.Api.Message;
using XOutput.Api.Serialization;
using XOutput.Core.Threading;

namespace XOutput.Devices.XInput
{
    public abstract class WebsocketClient
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private ClientWebSocket websocket;
        private CancellationTokenSource cancellationTokenSource;

        private readonly MessageReader messageReader;
        private readonly MessageWriter messageWriter;

        public WebsocketClient(MessageReader messageReader, MessageWriter messageWriter)
        {
            this.messageReader = messageReader;
            this.messageWriter = messageWriter;
        }

        public Task Start()
        {
            return Start("ws://192.168.1.2:8000/", DeviceTypes.MicrosoftXbox360, "ViGEm");
        }

        public async Task Start(string url, DeviceTypes deviceType, string emulator)
        {
            websocket = new ClientWebSocket();
            cancellationTokenSource = new CancellationTokenSource();
            string fullUrl = $"{url}{deviceType.ToString()}/{emulator}";
            await websocket.ConnectAsync(new Uri(fullUrl), cancellationTokenSource.Token);
            ThreadCreator.Create("Device emulator", HandleWebsocket).Start();
        }

        private async Task HandleWebsocket(CancellationToken cancellationToken)
        {
            while (websocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                string requestMessage = await ReadStringAsync(websocket, cancellationToken).ConfigureAwait(false);
                if (requestMessage == null)
                {
                    continue;
                }
                try
                {
                    var message = messageReader.ReadMessage(requestMessage);
                    ProcessMessage(message);
                }
                catch (Exception e)
                {
                    logger.Warn(e, "Error while handling websocket message: " + requestMessage);
                    continue;
                }
            }
            if (websocket.State != WebSocketState.Closed)
            {
                await websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Done", CancellationToken.None);
            }
        }

        protected abstract void ProcessMessage(MessageBase message);

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

        protected Task SendAsync(MessageBase message)
        {
            if (websocket.CloseStatus == null)
            {
                var data = Encoding.UTF8.GetBytes(messageWriter.WriteMessage(message));
                ArraySegment<byte> buffer = new ArraySegment<byte>(data);
                return websocket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationTokenSource.Token);
            }
            return Task.FromResult(0);
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
