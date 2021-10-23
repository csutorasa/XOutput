using Microsoft.AspNetCore.Http;
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
        private static readonly Regex PathRegex = new Regex($"/ws/{DeviceType}/([A-Za-z]+)");
        private readonly EmulatorService emulatorService;
        private readonly DeviceInfoService deviceInfoService;

        [ResolverMethod]
        public XboxWebSocketHandler(EmulatorService emulatorService, DeviceInfoService deviceInfoService)
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
            var emulator = emulatorService.FindEmulator<IXboxEmulator>(DeviceTypes.MicrosoftXbox360, emulatorName);
            var device = emulator.CreateXboxDevice();
            DeviceDisconnectedEvent disconnectedEvent = (sender, args) => closeFunction();
            device.Closed += disconnectedEvent;
            var ip = context.Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            deviceInfoService.Add(new NetworkDeviceInfo
            {
                Device = device,
                IPAddress = ip,
                DeviceType = DeviceTypes.MicrosoftXbox360,
                Emulator = emulator.Name,
            });
            return new List<IMessageHandler>
            {
                new XboxFeedbackResponseHandler(device, sendFunction.GetTyped<XboxFeedbackResponse>()),
                new XboxInputRequestHandler(device, disconnectedEvent),
            };
        }

        public void Close(IEnumerable<IMessageHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                if (handler is XboxInputRequestHandler)
                {
                    var device = (handler as XboxInputRequestHandler).device;
                    deviceInfoService.StopAndRemove(device.Id);
                }
                handler.Close();
            }
        }
    }
}
