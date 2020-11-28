using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Api.Message;
using XOutput.Api.Serialization;
using XOutput.Core.Threading;
using XOutput.Core.WebSocket;

namespace XOutput.Client.Websocket
{
    public abstract class WebsocketJsonClient
    {
        protected readonly MessageReader messageReader;
        protected readonly MessageWriter messageWriter;
        protected readonly WebSocketHelper webSocketHelper;
        protected readonly ClientWebSocket client;
        protected ThreadContext threadContext;
        private bool started = false;

        protected WebsocketJsonClient(MessageReader messageReader, MessageWriter messageWriter, WebSocketHelper webSocketHelper)
        {
            this.messageReader = messageReader;
            this.messageWriter = messageWriter;
            this.webSocketHelper = webSocketHelper;
            client = new ClientWebSocket();
        }

        protected WebsocketJsonClient(MessageReader messageReader, MessageWriter messageWriter, WebSocketHelper webSocketHelper, ClientWebSocket client)
        {
            this.messageReader = messageReader;
            this.messageWriter = messageWriter;
            this.webSocketHelper = webSocketHelper;
            this.client = client;
        }

        protected async Task Connect(Uri uri, CancellationToken token = default)
        {
            if (started)
            {
                return;
            }
            await client.ConnectAsync(uri, token);
            threadContext = ThreadCreator.CreateLoop("${asd}", ReadIncomingMessages, 0);
            started = true;
        }

        protected async Task Close(CancellationToken token = default)
        {
            if (!started)
            {
                return;
            }
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, null, token);
            threadContext.Cancel().Wait();
            started = false;
        }

        protected Task Send<T>(T message, CancellationToken token = default) where T : MessageBase
        {
            return webSocketHelper.SendStringAsync(client, messageWriter.GetString(message), Encoding.UTF8, token);
        }

        private async void ReadIncomingMessages(CancellationToken token)
        {
            string data = await webSocketHelper.ReadStringAsync(client, Encoding.UTF8, token);
            var message = messageReader.ReadString(data);
            ProcessMessage(message);
        }

        protected abstract void ProcessMessage(MessageBase message);
    }
}
