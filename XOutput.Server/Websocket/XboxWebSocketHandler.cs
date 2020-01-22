using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using XOutput.Api.Devices;
using XOutput.Api.Message;
using XOutput.Api.Message.Xbox;
using XOutput.Core.DependencyInjection;
using XOutput.Server.Emulation;

namespace XOutput.Server.Websocket.Xbox
{
    class XboxWebSocketHandler : IWebSocketHandler
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly EmulatorService emulatorService;

        [ResolverMethod]
        public XboxWebSocketHandler(EmulatorService emulatorService)
        {
            this.emulatorService = emulatorService;
        }

        public bool CanHandle(HttpListenerContext context)
        {
            return context.Request.Url.LocalPath.StartsWith("/microsoftxbox360/", StringComparison.InvariantCultureIgnoreCase);
        }

        public List<IMessageHandler> CreateHandlers(HttpListenerContext context, Func<MessageBase, Task> sendFunction)
        {
            string emulatorName = context.Request.Url.LocalPath.Replace("/microsoftxbox360/", "");
            var emulator = emulatorService.FindEmulator<IXboxEmulator>(DeviceTypes.MicrosoftXbox360, emulatorName);
            var device = emulator.CreateDevice();
            var handlers = new List<IMessageHandler>();
            handlers.Add(new DebugMessageHandler());
            handlers.Add(new XboxInputMessageHandler(device));
            return handlers;
        }
    }
}
