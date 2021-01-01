using System;
using System.Threading.Tasks;
using XOutput.Api.Message;
using XOutput.Api.Serialization;
using XOutput.Client.Websocket;
using XOutput.Core.WebSocket;
using XOutput.Message.Mappable;

namespace XOutput.Client.Input
{
    public class MappableDeviceClient : WebsocketJsonClient
    {
        public event FeedbackReceived FeedbackReceived;

        private readonly Uri uri;

        public MappableDeviceClient(MessageReader messageReader, MessageWriter messageWriter, WebSocketHelper webSocketHelper, string apiUrl) : base(messageReader, messageWriter, webSocketHelper)
        {
            uri = new Uri(apiUrl + "/ws/mappableDevice");
        }

        public async Task ConnectAsync(MappableDeviceDetailsMessage message)
        {
            await ConnectAsync(uri);
            await SendAsync(message);
        }

        protected override void ProcessMessage(MessageBase message)
        {
            if (message is MappableDeviceFeedbackMessage)
            {
                var feedbackMessage = message as MappableDeviceFeedbackMessage;
                FeedbackReceived?.Invoke(this, new FeedbackReceivedEventArgs
                {
                    SmallForceFeedback = feedbackMessage.SmallForceFeedback,
                    BigForceFeedback = feedbackMessage.BigForceFeedback,
                });
            }
        }

        protected Task SendInputAsync(MappableDeviceInputMessage message)
        {
            return SendAsync(message);
        }
    }

    public delegate void FeedbackReceived(object sender, FeedbackReceivedEventArgs args);

    public class FeedbackReceivedEventArgs
    {
        public double SmallForceFeedback { get; set; }
        public double BigForceFeedback { get; set; }
    }
}
