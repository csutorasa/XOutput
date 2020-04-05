using XOutput.Api.Message;
using XOutput.Api.Message.Xbox;
using XOutput.Server.Emulation;

namespace XOutput.Server.Websocket.Xbox
{
    class XboxInputMessageHandler : IMessageHandler
    {
        internal readonly XboxDevice device;
        private readonly DeviceDisconnectedEvent disconnectedEventHandler;

        public XboxInputMessageHandler(XboxDevice device, DeviceDisconnectedEvent disconnectedEventHandler)
        {
            this.device = device;
            this.disconnectedEventHandler = disconnectedEventHandler;
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
            device.Closed -= disconnectedEventHandler;
            device.Close();
        }
    }
}
