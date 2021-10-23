using System;
using System.Threading.Tasks;
using XOutput.Common;
using XOutput.Serialization;

namespace XOutput.Websocket.Xbox
{
    public class XboxDeviceClient : WebsocketJsonClient
    {
        public event XboxDeviceFeedbackReceived FeedbackReceived;

        public XboxDeviceClient(MessageReader messageReader, MessageWriter messageWriter, WebSocketHelper webSocketHelper, Uri baseUri) : base(messageReader, messageWriter, webSocketHelper, baseUri)
        {

        }

        public async Task ConnectAsync(string emulatorName)
        {
            await ConnectAsync($"ws/{DeviceTypes.MicrosoftXbox360.ToString()}/{emulatorName}");
        }

        protected override void ProcessMessage(MessageBase message)
        {
            if (message is XboxFeedbackResponse)
            {
                var feedbackMessage = message as XboxFeedbackResponse;
                FeedbackReceived?.Invoke(this, new XboxDeviceFeedbackReceivedEventArgs
                {
                    SmallForceFeedback = feedbackMessage.SmallForceFeedback,
                    BigForceFeedback = feedbackMessage.BigForceFeedback,
                });
            }
        }

        protected Task SendInputAsync(XboxInputRequest message)
        {
            return SendAsync(message);
        }
    }

    public delegate void XboxDeviceFeedbackReceived(object sender, XboxDeviceFeedbackReceivedEventArgs args);

    public class XboxDeviceFeedbackReceivedEventArgs
    {
        public double SmallForceFeedback { get; set; }
        public double BigForceFeedback { get; set; }
    }
}
