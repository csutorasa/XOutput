using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using XOutput.Api.Message.Input;
using XOutput.Core.DependencyInjection;
using XOutput.Devices.Input;

namespace XOutput.Server.Websocket.Input
{
    class InputWebSocketHandler : IWebSocketHandler
    {
        private readonly InputDeviceManager inputDeviceManager;

        [ResolverMethod]
        public InputWebSocketHandler(InputDeviceManager inputDeviceManager)
        {
            this.inputDeviceManager = inputDeviceManager;
        }

        public bool CanHandle(HttpContext context)
        {
            return context.Request.Path.Value.StartsWith($"/ws/input/");
        }

        public List<IMessageHandler> CreateHandlers(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction)
        {
            string deviceId = context.Request.Path.Value.Replace($"/ws/input/", "");
            var device = inputDeviceManager.FindInputDevice(deviceId);
            if (device == null)
            {
                throw new ArgumentException();
            }
            return new List<IMessageHandler>
            {
                new DebugMessageHandler(),
                new InputValuesMessageHandler(device, sendFunction.GetTyped<InputValuesMessage>()),
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
