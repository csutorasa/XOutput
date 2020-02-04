using System;
using System.Threading.Tasks;
using XOutput.Api.Message;
using XOutput.Api.Message.Xbox;
using XOutput.Api.Serialization;
using XOutput.Core.DependencyInjection;
using XOutput.Core.WebSocket;

namespace XOutput.Devices.XInput
{
    public class WebsocketXboxClient : WebsocketClient
    {
        public event Action<object, XboxFeedbackMessage> Feedback;

        [ResolverMethod(Scope.Prototype)]
        public WebsocketXboxClient(MessageReader messageReader, MessageWriter messageWriter, WebSocketHelper webSocketHelper) : base(messageReader, messageWriter, webSocketHelper)
        {

        }

        protected override void ProcessMessage(MessageBase message)
        {
            if (message.Type == XboxFeedbackMessage.MessageType)
            {
                var feedback = message as XboxFeedbackMessage;
                Feedback?.Invoke(this, feedback);
            }
        }

        public Task SendInput(XboxInputMessage message)
        {
            return SendAsync(message);
        }
    }
}
