using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XOutput.DependencyInjection;
using XOutput.Mapping.Input;

namespace XOutput.Websocket.Input
{
    class InputDeviceFeedbackWebSocketHandler : IWebSocketHandler
    {
        private static readonly Regex PathRegex = new Regex($"/ws/InputDevice/([-A-Za-z0-9]+)");
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

        public List<IMessageHandler> CreateHandlers(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction)
        {
            string id = PathRegex.Match(context.Request.Path.Value).Groups[1].Value;
            var device = inputDevices.Find(id);
            return new List<IMessageHandler>
            {
                new InputDeviceFeedbackHandler(device, sendFunction.GetTyped<InputDeviceInputResponse>()),
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
