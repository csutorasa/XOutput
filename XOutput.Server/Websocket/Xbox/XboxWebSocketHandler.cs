using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using XOutput.Common;
using XOutput.DependencyInjection;
using XOutput.Emulation;
using XOutput.Emulation.Xbox;

namespace XOutput.Websocket.Xbox
{
    class XboxWebSocketHandler : IWebSocketHandler
    {
        private static readonly string DeviceType = DeviceTypes.MicrosoftXbox360.ToString();
        private static readonly Regex PathRegex = new Regex($"/websocket/{DeviceType}/([A-Za-z]+)");
        private readonly EmulatorService emulatorService;
        private readonly EmulatedControllersService emulatedControllersService;

        [ResolverMethod]
        public XboxWebSocketHandler(EmulatorService emulatorService, EmulatedControllersService emulatedControllersService)
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
            var emulator = emulatorService.FindEmulator<IXboxEmulator>(DeviceTypes.MicrosoftXbox360, emulatorType);
            var device = emulator.CreateXboxDevice();
            DeviceDisconnectedEventHandler disconnectedEvent = (sender, args) => closeFunction();
            device.Closed += disconnectedEvent;
            var ip = context.Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            emulatedControllersService.Add(new NetworkDeviceInfo
            {
                Device = device,
                IPAddress = ip,
                DeviceType = DeviceTypes.MicrosoftXbox360,
                Emulator = emulator.Emulator,
            });
            return new XboxInputRequestHandler(closeFunction, sendFunction, device, emulatedControllersService, disconnectedEvent);
        }
    }
}
