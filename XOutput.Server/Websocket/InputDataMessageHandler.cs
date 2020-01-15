using NLog;
using XOutput.Api.Message;
using XOutput.Core.DependencyInjection;

namespace XOutput.Server.Websocket
{
    class InputDataMessageHandler : MessageHandler<InputDataMessage>
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        [ResolverMethod]
        public InputDataMessageHandler()
        {

        }

        public override void Handle(InputDataMessage message, WebsocketSessionContext context)
        {
            var inputs = message.Data;
            foreach (var input in inputs)
            {
                /*XInputTypes type;
                if (!Enum.TryParse(input.InputType, out type))
                {
                    logger.Error("Invalid input message: " + input);
                    continue;
                }
                device.Sources.OfType<WebXOutputSource>().First(s => s.XInputType == type).SetValue(input.Value);*/
            }
        }
    }
}
