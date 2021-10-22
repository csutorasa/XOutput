using System;
using XOutput.Api.Message;
using XOutput.Api.Message.Xbox;
using XOutput.Emulation.Xbox;

namespace XOutput.Server.Websocket.Xbox
{
    class XboxFeedbackMessageHandler : IMessageHandler
    {
        private readonly XboxDevice device;
        private readonly SenderFunction<XboxFeedbackMessage> senderFunction;

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
                SmallForceFeedback = args.Small,
                BigForceFeedback = args.Large,
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
            device.FeedbackEvent -= FeedbackEvent;
        }
    }
}
