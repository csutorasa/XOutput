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
using XOutput.Core.WebSocket;

namespace XOutput.Devices.XInput
{
    public abstract class WebsocketClient
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private ClientWebSocket websocket;
        private CancellationTokenSource cancellationTokenSource;

        private readonly MessageReader messageReader;
        private readonly MessageWriter messageWriter;
        private readonly WebSocketHelper webSocketHelper;

        public WebsocketClient(MessageReader messageReader, MessageWriter messageWriter, WebSocketHelper webSocketHelper)
        {
            this.messageReader = messageReader;
            this.messageWriter = messageWriter;
            this.webSocketHelper = webSocketHelper;
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
                string requestMessage = await webSocketHelper.ReadStringAsync(websocket, Encoding.UTF8, cancellationToken);
                if (requestMessage == null)
                {
                    continue;
                }
                try
                {
                    var message = messageReader.ReadString(requestMessage);
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

        protected Task SendAsync(MessageBase message)
        {
            return webSocketHelper.SendStringAsync(websocket, messageWriter.GetString(message), Encoding.UTF8, cancellationTokenSource.Token);
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
