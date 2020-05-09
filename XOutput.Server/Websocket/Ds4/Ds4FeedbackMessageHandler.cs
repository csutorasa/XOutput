using System;
using XOutput.Api.Message;
using XOutput.Api.Message.Ds4;
using XOutput.Emulation.Ds4;

namespace XOutput.Server.Websocket.Ds4
{
    class Ds4FeedbackMessageHandler : IMessageHandler
    {
        private readonly Ds4Device device;
        private readonly SenderFunction<Ds4FeedbackMessage> senderFunction;

        public Ds4FeedbackMessageHandler(Ds4Device device, SenderFunction<Ds4FeedbackMessage> senderFunction)
        {
            this.device = device;
            this.senderFunction = senderFunction;
            device.FeedbackEvent += FeedbackEvent;
        }

        private void FeedbackEvent(object sender, Ds4FeedbackEventArgs args)
        {
            senderFunction(new Ds4FeedbackMessage
            {
                Small = args.Small,
                Large = args.Large,
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
