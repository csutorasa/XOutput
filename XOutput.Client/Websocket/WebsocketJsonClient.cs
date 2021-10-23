using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Serialization;
using XOutput.Threading;

namespace XOutput.Websocket
{
    public abstract class WebsocketJsonClient
    {
        protected readonly MessageReader messageReader;
        protected readonly MessageWriter messageWriter;
        protected readonly WebSocketHelper webSocketHelper;
        protected readonly ClientWebSocket client;
        protected readonly Uri baseUri;
        protected ThreadContext threadContext;
        private bool started = false;

        protected WebsocketJsonClient(MessageReader messageReader, MessageWriter messageWriter, WebSocketHelper webSocketHelper, Uri baseUri)
        {
            this.messageReader = messageReader;
            this.messageWriter = messageWriter;
            this.webSocketHelper = webSocketHelper;
            this.baseUri = baseUri;
            client = new ClientWebSocket();
        }

        protected WebsocketJsonClient(MessageReader messageReader, MessageWriter messageWriter, WebSocketHelper webSocketHelper, Uri baseUri, ClientWebSocket client)
        {
            this.messageReader = messageReader;
            this.messageWriter = messageWriter;
            this.webSocketHelper = webSocketHelper;
            this.baseUri = baseUri;
            this.client = client;
        }

        protected async Task ConnectAsync(string path, CancellationToken token = default)
        {
            if (started)
            {
                return;
            }
            await client.ConnectAsync(new Uri(baseUri.ToString() + path), token);
            threadContext = ThreadCreator.CreateLoop("Websocket client", async (token) => await ReadIncomingMessagesAsync(token), 0);
            started = true;
        }

        public async Task CloseAsync(CancellationToken token = default)
        {
            if (!started)
            {
                return;
            }
            await client.CloseAsync(WebSocketCloseStatus.NormalClosure, null, token);
            threadContext.Cancel().Wait();
            started = false;
        }

        protected Task SendAsync<T>(T message, CancellationToken token = default) where T : MessageBase
        {
            return webSocketHelper.SendStringAsync(client, messageWriter.GetString(message), Encoding.UTF8, token);
        }

        private async Task ReadIncomingMessagesAsync(CancellationToken token)
        {
            string data = await webSocketHelper.ReadStringAsync(client, Encoding.UTF8, token);
            var message = messageReader.ReadString(data);
            ProcessMessage(message);
        }

        protected abstract void ProcessMessage(MessageBase message);
    }
}
