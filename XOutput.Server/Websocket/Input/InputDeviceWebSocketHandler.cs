using Microsoft.AspNetCore.Http;
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

        public IMessageHandler CreateHandler(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction)
        {
            return new InputDeviceMessageHandler(closeFunction, sendFunction, inputDevices);
        }
    }
}
