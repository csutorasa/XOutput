using XOutput.Api.Message;
using XOutput.Api.Message.Ds4;
using XOutput.Server.Emulation;

namespace XOutput.Server.Websocket.Ds4
{
    class Ds4InputMessageHandler : IMessageHandler
    {
        internal readonly Ds4Device device;
        private readonly DeviceDisconnectedEvent disconnectedEventHandler;

        public Ds4InputMessageHandler(Ds4Device device, DeviceDisconnectedEvent disconnectedEventHandler)
        {
            this.device = device;
            this.disconnectedEventHandler = disconnectedEventHandler;
        }

        public bool CanHandle(MessageBase message)
        {
            return message is Ds4InputMessage;
        }

        public void Handle(MessageBase message)
        {
            device.SendInput(message as Ds4InputMessage);
        }

        public void Close()
        {
            device.Closed -= disconnectedEventHandler;
            device.Close();
        }
    }
}
