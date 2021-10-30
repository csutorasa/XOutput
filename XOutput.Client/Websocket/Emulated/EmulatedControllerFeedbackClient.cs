using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XOutput.Serialization;

namespace XOutput.Websocket.Emulated
{
    public class EmulatedControllerFeedbackClient : WebsocketJsonClient
    {
        public event InputDeviceFeedbackReceived FeedbackReceived;

        public EmulatedControllerFeedbackClient(MessageReader messageReader, MessageWriter messageWriter, WebSocketHelper webSocketHelper, Uri baseUri) : base(messageReader, messageWriter, webSocketHelper, baseUri)
        {

        }

        public async Task ConnectAsync(string id)
        {
            await base.ConnectAsync("EmulatedConttroller/" + id);
        }

        protected override void ProcessMessage(MessageBase message)
        {
            if (message is EmulatedControllerInputResponse)
            {
                var feedbackMessage = message as EmulatedControllerInputResponse;
                FeedbackReceived?.Invoke(this, new InputDeviceFeedbackReceivedEventArgs
                {
                    InputValues = feedbackMessage.Sources.Select(s => new EmulatedControllerSourceValue { Id = s.Id, Value = s.Value }).ToList(),
                    ForceFeedbacks = feedbackMessage.Targets.Select(t => new EmulatedControllerTargetValue { Id = t.Id, Value = t.Value }).ToList(),
                });
            }
        }
    }

    public delegate void InputDeviceFeedbackReceived(object sender, InputDeviceFeedbackReceivedEventArgs args);

    public class InputDeviceFeedbackReceivedEventArgs
    {
        public List<EmulatedControllerSourceValue> InputValues { get; set; }
        public List<EmulatedControllerTargetValue> ForceFeedbacks { get; set; }
    }
}
