using System;
using System.Threading.Tasks;
using XOutput.Common;
using XOutput.Serialization;

namespace XOutput.Websocket.Ds4
{
    public class DS4DeviceClient : WebsocketJsonClient
    {
        public event DS4DeviceFeedbackReceived FeedbackReceived;

        public DS4DeviceClient(MessageReader messageReader, MessageWriter messageWriter, WebSocketHelper webSocketHelper, Uri baseUri) : base(messageReader, messageWriter, webSocketHelper, baseUri)
        {

        }

        public async Task ConnectAsync(string emulatorName)
        {
            await base.ConnectAsync($"ws/{DeviceTypes.SonyDualShock4.ToString()}/{emulatorName}");
        }

        protected override void ProcessMessage(MessageBase message)
        {
            if (message is Ds4FeedbackResponse)
            {
                var feedbackMessage = message as Ds4FeedbackResponse;
                FeedbackReceived?.Invoke(this, new DS4DeviceFeedbackReceivedEventArgs
                {
                    SmallForceFeedback = feedbackMessage.SmallForceFeedback,
                    BigForceFeedback = feedbackMessage.BigForceFeedback,
                });
            }
        }

        protected Task SendInputAsync(Ds4InputRequest message)
        {
            return SendAsync(message);
        }
    }

    public delegate void DS4DeviceFeedbackReceived(object sender, DS4DeviceFeedbackReceivedEventArgs args);

    public class DS4DeviceFeedbackReceivedEventArgs
    {
        public double SmallForceFeedback { get; set; }
        public double BigForceFeedback { get; set; }
    }
}
