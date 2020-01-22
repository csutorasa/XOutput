using NLog;
using System.Net;
using XOutput.Api.Message;
using XOutput.Core.DependencyInjection;

namespace XOutput.Server.Websocket
{
    class DebugMessageHandler : IMessageHandler
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public DebugMessageHandler()
        {

        }

        public bool CanHandle(MessageBase message)
        {
            return message is DebugMessage;
        }

        public void Handle(MessageBase message)
        {
            var debugMessage = message as DebugMessage;
            logger.Info("Message from client: " + debugMessage.Data);
        }

        public void Close()
        {

        }
    }
}
