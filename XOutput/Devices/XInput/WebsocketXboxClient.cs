using NLog;
using System;
using System.Threading.Tasks;
using XOutput.Api.Message;
using XOutput.Api.Message.Xbox;
using XOutput.Api.Serialization;
using XOutput.Core.DependencyInjection;

namespace XOutput.Devices.XInput
{
    public class WebsocketXboxClient : WebsocketClient
    {
        public event Action<object, XboxFeedbackMessage> Feedback;

        [ResolverMethod(Scope.Prototype)]
        public WebsocketXboxClient(MessageReader messageReader, MessageWriter messageWriter) : base(messageReader, messageWriter)
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
