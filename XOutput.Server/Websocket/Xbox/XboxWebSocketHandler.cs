using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using XOutput.DependencyInjection;
using XOutput.Emulation;
using XOutput.Emulation.Xbox;
using XOutput.Websocket.Common;

namespace XOutput.Websocket.Xbox
{
    class XboxWebSocketHandler : IWebSocketHandler
    {
        private static readonly string DeviceType = DeviceTypes.MicrosoftXbox360.ToString();
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
            return context.Request.Path.Value.StartsWith($"/ws/{DeviceType}/");
        }

        public List<IMessageHandler> CreateHandlers(HttpContext context, CloseFunction closeFunction, SenderFunction sendFunction)
        {
            string emulatorName = context.Request.Path.Value.Replace($"/ws/{DeviceType}/", "");
            var emulator = emulatorService.FindEmulator<IXboxEmulator>(XOutput.Emulation.DeviceTypes.MicrosoftXbox360, emulatorName);
            var device = emulator.CreateXboxDevice();
            DeviceDisconnectedEvent disconnectedEvent = (sender, args) => closeFunction();
            device.Closed += disconnectedEvent;
            var ip = context.Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            deviceInfoService.Add(new NetworkDeviceInfo
            {
                Device = device,
                IPAddress = ip,
                DeviceType = XOutput.Emulation.DeviceTypes.MicrosoftXbox360,
                Emulator = emulator.Name,
            });
            return new List<IMessageHandler>
            {
                new DebugMessageHandler(),
                new XboxFeedbackMessageHandler(device, sendFunction.GetTyped<XboxFeedbackResponse>()),
                new XboxInputMessageHandler(device, disconnectedEvent),
            };
        }

        public void Close(IEnumerable<IMessageHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                if (handler is XboxInputMessageHandler)
                {
                    var device = (handler as XboxInputMessageHandler).device;
                    deviceInfoService.StopAndRemove(device.Id);
                }
                handler.Close();
            }
        }
    }
}
