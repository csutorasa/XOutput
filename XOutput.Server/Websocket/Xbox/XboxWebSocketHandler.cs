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
        private readonly EmulatorService emulatorService;

        [ResolverMethod]
        public XboxWebSocketHandler(EmulatorService emulatorService)
        {
            this.emulatorService = emulatorService;
        }

        public bool CanHandle(HttpListenerContext context)
        {
            return context.Request.Url.LocalPath.StartsWith("/MicrosoftXbox360/");
        }

        public List<IMessageHandler> CreateHandlers(HttpListenerContext context, SenderFunction sendFunction)
        {
            string emulatorName = context.Request.Url.LocalPath.Replace("/MicrosoftXbox360/", "");
            var emulator = emulatorService.FindEmulator<IXboxEmulator>(DeviceTypes.MicrosoftXbox360, emulatorName);
            var device = emulator.CreateDevice();
            return new List<IMessageHandler>
            {
                new DebugMessageHandler(),
                new XboxFeedbackMessageHandler(device, sendFunction.GetTyped<XboxFeedbackMessage>()),
                new XboxInputMessageHandler(device),
            };
        }
    }
}
