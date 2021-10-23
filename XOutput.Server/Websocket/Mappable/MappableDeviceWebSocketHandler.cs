using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using XOutput.DependencyInjection;
using XOutput.Mapping.Input;
using XOutput.Message.Mappable;
using XOutput.Websocket.Common;

namespace XOutput.Websocket.Mappable
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
                new MappableDeviceMessageHandler(mappableDevices, sendFunction.GetTyped<MappableDeviceFeedbackResponse>()),
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
