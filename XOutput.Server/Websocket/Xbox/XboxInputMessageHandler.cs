using NLog;
using System;
using System.Net;
using XOutput.Api.Devices;
using XOutput.Api.Message;
using XOutput.Api.Message.Xbox;
using XOutput.Core.DependencyInjection;
using XOutput.Server.Emulation;

namespace XOutput.Server.Websocket.Xbox
{
    class XboxInputMessageHandler : IMessageHandler
    {
        private XboxDevice device;

        public XboxInputMessageHandler(XboxDevice device)
        {
            this.device = device;
        }

        public bool CanHandle(MessageBase message)
        {
            return message is XboxInputMessage;
        }

        public void Handle(MessageBase message)
        {
            device.SendInput(message as XboxInputMessage);
        }

        public void Close()
        {
            device.Close();
        }
    }
}
