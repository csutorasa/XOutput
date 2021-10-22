using System;
using System.Threading.Tasks;
using XOutput.Api.Devices;
using XOutput.Api.Message;
using XOutput.Api.Message.Xbox;
using XOutput.Api.Serialization;
using XOutput.Client.Websocket;
using XOutput.Core.WebSocket;

namespace XOutput.Client.Input
{
    public class XboxDeviceClient : WebsocketJsonClient
    {
        public event XboxDeviceFeedbackReceived FeedbackReceived;

        public XboxDeviceClient(MessageReader messageReader, MessageWriter messageWriter, WebSocketHelper webSocketHelper, Uri baseUri) : base(messageReader, messageWriter, webSocketHelper, baseUri)
        {

        }

        public async Task ConnectAsync(string emulatorName)
        {
            await ConnectAsync($"ws/${DeviceTypes.MicrosoftXbox360.ToString()}/${emulatorName}");
        }

        protected override void ProcessMessage(MessageBase message)
        {
            if (message is XboxFeedbackMessage)
            {
                var feedbackMessage = message as XboxFeedbackMessage;
                FeedbackReceived?.Invoke(this, new XboxDeviceFeedbackReceivedEventArgs
                {
                    SmallForceFeedback = feedbackMessage.SmallForceFeedback,
                    BigForceFeedback = feedbackMessage.BigForceFeedback,
                });
            }
        }

        protected Task SendInputAsync(XboxInputMessage message)
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
