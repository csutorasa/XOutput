using NLog;

namespace XOutput.Websocket.Common
{
    class DebugMessageHandler : IMessageHandler
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public DebugMessageHandler()
        {

        }

        public bool CanHandle(MessageBase message)
        {
            return message is DebugRequest;
        }

        public void Handle(MessageBase message)
        {
            var debugMessage = message as DebugRequest;
            logger.Info("Message from client: " + debugMessage.Data);
        }

        public void Close()
        {
            // There is nothing to clean up
        }
    }
}
