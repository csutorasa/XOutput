using System;
using System.Threading.Tasks;
using XOutput.Message.Mappable;
using XOutput.Serialization;

namespace XOutput.Websocket.Mappable
{
    public class MappableDeviceClient : WebsocketJsonClient
    {
        public event MappableDeviceFeedbackReceived FeedbackReceived;

        public MappableDeviceClient(MessageReader messageReader, MessageWriter messageWriter, WebSocketHelper webSocketHelper, Uri baseUri) : base(messageReader, messageWriter, webSocketHelper, baseUri)
        {

        }

        public async Task ConnectAsync(MappableDeviceDetailsRequest message)
        {
            await ConnectAsync("mappableDevice");
            await SendAsync(message);
        }

        protected override void ProcessMessage(MessageBase message)
        {
            if (message is MappableDeviceFeedbackResponse)
            {
                var feedbackMessage = message as MappableDeviceFeedbackResponse;
                FeedbackReceived?.Invoke(this, new MappableDeviceFeedbackReceivedEventArgs
                {
                    SmallForceFeedback = feedbackMessage.SmallForceFeedback,
                    BigForceFeedback = feedbackMessage.BigForceFeedback,
                });
            }
        }

        protected Task SendInputAsync(MappableDeviceInputRequest message)
        {
            return SendAsync(message);
        }
    }

    public delegate void MappableDeviceFeedbackReceived(object sender, MappableDeviceFeedbackReceivedEventArgs args);

    public class MappableDeviceFeedbackReceivedEventArgs
    {
        public double SmallForceFeedback { get; set; }
        public double BigForceFeedback { get; set; }
    }
}
