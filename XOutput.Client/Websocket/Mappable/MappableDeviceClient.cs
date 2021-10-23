using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XOutput.Serialization;

namespace XOutput.Websocket.Input
{
    public class InputDeviceClient : WebsocketJsonClient
    {
        public event InputDeviceFeedbackReceived FeedbackReceived;

        public InputDeviceClient(MessageReader messageReader, MessageWriter messageWriter, WebSocketHelper webSocketHelper, Uri baseUri) : base(messageReader, messageWriter, webSocketHelper, baseUri)
        {

        }

        public async Task ConnectAsync(InputDeviceDetailsRequest message)
        {
            await ConnectAsync("InputDevice");
            await SendAsync(message);
        }

        protected override void ProcessMessage(MessageBase message)
        {
            if (message is InputDeviceFeedbackResponse)
            {
                var feedbackMessage = message as InputDeviceFeedbackResponse;
                FeedbackReceived?.Invoke(this, new InputDeviceFeedbackReceivedEventArgs
                {
                    ForceFeedbacks = feedbackMessage.Targets.Select(t => new InputDeviceTargetValue { Id = t.Id, Value = t.Value }).ToList(),
                });
            }
        }

        protected Task SendInputAsync(InputDeviceInputRequest message)
        {
            return SendAsync(message);
        }
    }

    public delegate void InputDeviceFeedbackReceived(object sender, InputDeviceFeedbackReceivedEventArgs args);

    public class InputDeviceFeedbackReceivedEventArgs
    {
        public List<InputDeviceTargetValue> ForceFeedbacks { get; set; }
    }
}
