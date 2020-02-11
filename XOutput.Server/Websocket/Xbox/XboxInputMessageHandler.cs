using XOutput.Api.Message;
using XOutput.Api.Message.Xbox;
using XOutput.Server.Emulation;

namespace XOutput.Server.Websocket.Xbox
{
    class XboxInputMessageHandler : IMessageHandler
    {
        private readonly XboxDevice device;

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
