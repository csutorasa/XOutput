using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XOutput.DependencyInjection;
using XOutput.Emulation;
using XOutput.Emulation.Ds4;
using XOutput.Websocket.Common;

namespace XOutput.Websocket.Ds4
{
    class Ds4WebSocketHandler : IWebSocketHandler
    {
        private static readonly string DeviceType = DeviceTypes.SonyDualShock4.ToString();
        private static readonly Regex PathRegex = new Regex($"/ws/{DeviceType}/([A-Za-z]+)");
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
            string path = context.Request.Path.Value;
            return PathRegex.IsMatch(path);
        }

        public List<IMessageHandler> CreateHandlers(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction)
        {
            string emulatorName = PathRegex.Match(context.Request.Path.Value).Groups[1].Value;
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
                new Ds4FeedbackResponseHandler(device, sendFunction.GetTyped<Ds4FeedbackResponse>()),
                new Ds4InputRequestHandler(device, disconnectedEvent),
            };
        }

        public void Close(IEnumerable<IMessageHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                if (handler is Ds4InputRequestHandler)
                {
                    var device = (handler as Ds4InputRequestHandler).device;
                    deviceInfoService.StopAndRemove(device.Id);
                }
                handler.Close();
            }
        }
    }
}
