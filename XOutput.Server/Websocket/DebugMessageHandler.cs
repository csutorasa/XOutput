using NLog;
using XOutput.Api.Message;
using XOutput.Core.DependencyInjection;

namespace XOutput.Server.Websocket
{
    class DebugMessageHandler : MessageHandler<DebugMessage>
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        [ResolverMethod]
        public DebugMessageHandler()
        {

        }

        public override void Handle(DebugMessage message, WebsocketSessionContext context)
        {
            logger.Info("Message from client: " + message.Data);
        }
    }
}
