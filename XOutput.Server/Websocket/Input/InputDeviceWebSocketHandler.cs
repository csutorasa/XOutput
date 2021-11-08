using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using XOutput.DependencyInjection;
using XOutput.Mapping.Input;

namespace XOutput.Websocket.Input
{
    class InputDeviceWebSocketHandler : IWebSocketHandler
    {
        private readonly InputDevices inputDevices;

        [ResolverMethod]
        public InputDeviceWebSocketHandler(InputDevices inputDevices)
        {
            this.inputDevices = inputDevices;
        }

        public bool CanHandle(HttpContext context)
        {
            return context.Request.Path.Value == $"{IWebSocketHandler.WebsocketBasePath}/InputDevice";
        }

        public List<IMessageHandler> CreateHandlers(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction)
        {
            return new List<IMessageHandler>
            {
                new InputDeviceMessageHandler(inputDevices, sendFunction.GetTyped<InputDeviceFeedbackResponse>()),
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
