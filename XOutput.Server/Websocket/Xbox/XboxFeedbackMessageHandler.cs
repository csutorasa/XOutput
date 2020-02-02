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
    class XboxFeedbackMessageHandler : IMessageHandler
    {
        private XboxDevice device;
        private SenderFunction<XboxFeedbackMessage> senderFunction;

        public XboxFeedbackMessageHandler(XboxDevice device, SenderFunction<XboxFeedbackMessage> senderFunction)
        {
            this.device = device;
            this.senderFunction = senderFunction;
            device.FeedbackEvent += FeedbackEvent;
        }

        private void FeedbackEvent(object sender, XboxFeedbackEventArgs args)
        {
            senderFunction(new XboxFeedbackMessage
            {
                Small = args.Small,
                Large = args.Large,
                LedNumber = args.LedNumber,
            });
        }

        public bool CanHandle(MessageBase message)
        {
            return false;
        }

        public void Handle(MessageBase message)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {

        }
    }
}
