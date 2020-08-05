using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using XOutput.Api.Devices;
using XOutput.Api.Message.Ds4;
using XOutput.Core.DependencyInjection;
using XOutput.Emulation;
using XOutput.Emulation.Ds4;

namespace XOutput.Server.Websocket.Ds4
{
    class Ds4WebSocketHandler : IWebSocketHandler
    {
        private static readonly string DeviceType = Api.Devices.DeviceTypes.SonyDualShock4.ToString();
        private readonly EmulatorService emulatorService;
        private readonly DeviceInfoService deviceInfoService;

        [ResolverMethod]
        public Ds4WebSocketHandler(EmulatorService emulatorService, DeviceInfoService deviceInfoService)
        {
            this.emulatorService = emulatorService;
            this.deviceInfoService = deviceInfoService;
        }

        public bool CanHandle(HttpContext context)
        {
            return context.Request.Path.Value.StartsWith($"/ws/{DeviceType}/");
        }

        public List<IMessageHandler> CreateHandlers(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction)
        {
            string emulatorName = context.Request.Path.Value.Replace($"/ws/{DeviceType}/", "");
            var emulator = emulatorService.FindEmulator<IDs4Emulator>(XOutput.Emulation.DeviceTypes.SonyDualShock4, emulatorName);
            var device = emulator.CreateDs4Device();
            DeviceDisconnectedEvent disconnectedEvent = (sender, args) => closeFunction();
            device.Closed += disconnectedEvent;
            var ip = context.Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            deviceInfoService.Add(new NetworkDeviceInfo
            {
                Device = device,
                IPAddress = ip,
                DeviceType = XOutput.Emulation.DeviceTypes.SonyDualShock4,
                Emulator = emulator.Name,
            });
            return new List<IMessageHandler>
            {
                new DebugMessageHandler(),
                new Ds4FeedbackMessageHandler(device, sendFunction.GetTyped<Ds4FeedbackMessage>()),
                new Ds4InputMessageHandler(device, disconnectedEvent),
            };
        }

        public void Close(IEnumerable<IMessageHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                if (handler is Ds4InputMessageHandler)
                {
                    var device = (handler as Ds4InputMessageHandler).device;
                    deviceInfoService.StopAndRemove(device.Id);
                }
                handler.Close();
            }
        }
    }
}
