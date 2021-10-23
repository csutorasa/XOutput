using System;
using XOutput.Emulation.Xbox;

namespace XOutput.Websocket.Xbox
{
    class XboxFeedbackMessageHandler : IMessageHandler
    {
        private readonly XboxDevice device;
        private readonly SenderFunction<XboxFeedbackResponse> senderFunction;

        public XboxFeedbackMessageHandler(XboxDevice device, SenderFunction<XboxFeedbackResponse> senderFunction)
        {
            this.device = device;
            this.senderFunction = senderFunction;
            device.FeedbackEvent += FeedbackEvent;
        }

        private void FeedbackEvent(object sender, XboxFeedbackEventArgs args)
        {
            senderFunction(new XboxFeedbackResponse
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
