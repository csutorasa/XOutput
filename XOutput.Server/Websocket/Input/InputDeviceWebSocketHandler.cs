using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using XOutput.DependencyInjection;
using XOutput.Mapping.Input;

namespace XOutput.Websocket.Input
{
    class InputDeviceWebSocketHandler : IWebSocketHandler
    {
        private readonly InputDevices InputDevices;

        [ResolverMethod]
        public InputDeviceWebSocketHandler(InputDevices InputDevices)
        {
            this.InputDevices = InputDevices;
        }

        public bool CanHandle(HttpContext context)
        {
            return context.Request.Path.Value == "/ws/InputDevice";
        }

        public List<IMessageHandler> CreateHandlers(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction)
        {
            return new List<IMessageHandler>
            {
                new InputDeviceMessageHandler(InputDevices, sendFunction.GetTyped<InputDeviceFeedbackResponse>()),
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
