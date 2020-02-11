using NLog;
using XOutput.Api.Message;

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
            // There is nothing to clean up
        }
    }
}
