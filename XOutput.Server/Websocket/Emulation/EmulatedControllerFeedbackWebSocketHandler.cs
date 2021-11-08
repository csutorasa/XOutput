using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using XOutput.DependencyInjection;
using XOutput.Mapping.Controller;

namespace XOutput.Websocket.Emulation
{
    class EmulatedControllerFeedbackWebSocketHandler : IWebSocketHandler
    {
        private static readonly Regex PathRegex = new Regex($"{IWebSocketHandler.WebsocketBasePath}/EmulatedController/([-A-Za-z0-9]+)");
        private readonly MappedControllers emulatedControllers;

        [ResolverMethod]
        public EmulatedControllerFeedbackWebSocketHandler(MappedControllers emulatedControllers)
        {
            this.emulatedControllers = emulatedControllers;
        }

        public bool CanHandle(HttpContext context)
        {
            return PathRegex.IsMatch(context.Request.Path.Value);
        }

        public IMessageHandler CreateHandler(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction)
        {
            string id = PathRegex.Match(context.Request.Path.Value).Groups[1].Value;
            var emulatedController = emulatedControllers.Find(id);
            return new EmulatedControllerFeedbackHandler(closeFunction, sendFunction, emulatedController);
        }
    }
}
