using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using XOutput.Core.DependencyInjection;
using XOutput.Mapping.Input;
using XOutput.Message.Mappable;

namespace XOutput.Server.Websocket.Mappable
{
    class MappableDeviceWebSocketHandler : IWebSocketHandler
    {
        private readonly MappableDevices mappableDevices;

        [ResolverMethod]
        public MappableDeviceWebSocketHandler(MappableDevices mappableDevices)
        {
            this.mappableDevices = mappableDevices;
        }

        public bool CanHandle(HttpContext context)
        {
            return context.Request.Path.Value.StartsWith($"/ws/mappableDevice");
        }

        public List<IMessageHandler> CreateHandlers(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction)
        {
            return new List<IMessageHandler>
            {
                new DebugMessageHandler(),
                new MappableDeviceMessageHandler(mappableDevices, sendFunction.GetTyped<MappableDeviceFeedbackMessage>()),
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
