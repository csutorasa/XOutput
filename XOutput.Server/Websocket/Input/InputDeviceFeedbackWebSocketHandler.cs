using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;
using XOutput.DependencyInjection;
using XOutput.Mapping.Input;

namespace XOutput.Websocket.Input
{
    class InputDeviceFeedbackWebSocketHandler : IWebSocketHandler
    {
        private static readonly Regex PathRegex = new Regex($"{IWebSocketHandler.WebsocketBasePath}/InputDevice/([-A-Za-z0-9]+)");
        private readonly InputDevices inputDevices;

        [ResolverMethod]
        public InputDeviceFeedbackWebSocketHandler(InputDevices inputDevices)
        {
            this.inputDevices = inputDevices;
        }

        public bool CanHandle(HttpContext context)
        {
            return PathRegex.IsMatch(context.Request.Path.Value);
        }

        public IMessageHandler CreateHandler(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction)
        {
            string id = PathRegex.Match(context.Request.Path.Value).Groups[1].Value;
            var device = inputDevices.Find(id);
            return new InputDeviceFeedbackHandler(closeFunction, sendFunction, device);
        }
    }
}
