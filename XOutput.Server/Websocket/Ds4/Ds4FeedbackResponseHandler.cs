using System;
using XOutput.Emulation.Ds4;

namespace XOutput.Websocket.Ds4
{
    class Ds4FeedbackResponseHandler : IMessageHandler
    {
        private readonly Ds4Device device;
        private readonly SenderFunction<Ds4FeedbackResponse> senderFunction;

        public Ds4FeedbackResponseHandler(Ds4Device device, SenderFunction<Ds4FeedbackResponse> senderFunction)
        {
            this.device = device;
            this.senderFunction = senderFunction;
            device.FeedbackEvent += FeedbackEvent;
        }

        private void FeedbackEvent(object sender, Ds4FeedbackEventArgs args)
        {
            senderFunction(new Ds4FeedbackResponse
            {
                SmallForceFeedback = args.Small,
                BigForceFeedback = args.Large,
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
