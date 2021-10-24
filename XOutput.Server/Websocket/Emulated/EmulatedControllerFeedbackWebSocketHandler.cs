using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XOutput.DependencyInjection;
using XOutput.Mapping.Controller;
using XOutput.Mapping.Input;

namespace XOutput.Websocket.Emulated
{
    class EmulatedControllerFeedbackWebSocketHandler : IWebSocketHandler
    {
        private static readonly Regex PathRegex = new Regex($"/ws/InputDevice/([-A-Za-z0-9]+)");
        private readonly EmulatedControllers emulatedControllers;

        [ResolverMethod]
        public EmulatedControllerFeedbackWebSocketHandler(EmulatedControllers emulatedControllers)
        {
            this.emulatedControllers = emulatedControllers;
        }

        public bool CanHandle(HttpContext context)
        {
            return PathRegex.IsMatch(context.Request.Path.Value);
        }

        public List<IMessageHandler> CreateHandlers(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction)
        {
            string id = PathRegex.Match(context.Request.Path.Value).Groups[1].Value;
            var emulatedController = emulatedControllers.Find(id);
            return new List<IMessageHandler>
            {
                new EmulatedControllerFeedbackHandler(emulatedController, sendFunction.GetTyped<EmulatedControllerInputResponse>()),
            };
        }

        public void Close(IEnumerable<IMessageHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                handler.Close();
            }
        }
    }
}
