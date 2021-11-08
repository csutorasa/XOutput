using Microsoft.AspNetCore.Http;
using System;
using System.Text.RegularExpressions;
using XOutput.Common;
using XOutput.DependencyInjection;
using XOutput.Emulation;
using XOutput.Emulation.Ds4;

namespace XOutput.Websocket.Ds4
{
    class Ds4WebSocketHandler : IWebSocketHandler
    {
        private static readonly string DeviceType = DeviceTypes.SonyDualShock4.ToString();
        private static readonly Regex PathRegex = new Regex($"/websocket/{DeviceType}/([A-Za-z]+)");
        private readonly EmulatorService emulatorService;
        private readonly EmulatedControllersService emulatedControllersService;

        [ResolverMethod]
        public Ds4WebSocketHandler(EmulatorService emulatorService, EmulatedControllersService emulatedControllersService)
        {
            this.emulatorService = emulatorService;
            this.emulatedControllersService = emulatedControllersService;
        }

        public bool CanHandle(HttpContext context)
        {
            string path = context.Request.Path.Value;
            return PathRegex.IsMatch(path);
        }

        public IMessageHandler CreateHandler(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction)
        {
            string emulatorName = PathRegex.Match(context.Request.Path.Value).Groups[1].Value;
            Emulators emulatorType = Enum.Parse<Emulators>(emulatorName);
            var emulator = emulatorService.FindEmulator<IDs4Emulator>(DeviceTypes.SonyDualShock4, emulatorType);
            var device = emulator.CreateDs4Device();
            DeviceDisconnectedEventHandler disconnectedEvent = (sender, args) => closeFunction();
            device.Closed += disconnectedEvent;
            var ip = context.Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            emulatedControllersService.Add(new NetworkDeviceInfo
            {
                Device = device,
                IPAddress = ip,
                DeviceType = DeviceTypes.SonyDualShock4,
                Emulator = emulator.Emulator,
            });
            return new Ds4InputRequestHandler(closeFunction, sendFunction, device, emulatedControllersService, disconnectedEvent);
        }
    }
}
