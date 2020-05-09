using XOutput.Api.Message;
using XOutput.Api.Message.Xbox;
using XOutput.Emulation;
using XOutput.Emulation.Xbox;

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
            var inputMessage = message as XboxInputMessage;
            device.SendInput(new XboxInput
            {
                A = inputMessage.A,
                B = inputMessage.B,
                X = inputMessage.X,
                Y = inputMessage.Y,
                L1 = inputMessage.L1,
                L3 = inputMessage.L3,
                R1 = inputMessage.R1,
                R3 = inputMessage.R3,
                Start = inputMessage.Start,
                Back = inputMessage.Back,
                Home = inputMessage.Home,
                UP = inputMessage.UP,
                DOWN = inputMessage.DOWN,
                LEFT = inputMessage.LEFT,
                RIGHT = inputMessage.RIGHT,
                LX = inputMessage.LX,
                LY = inputMessage.LY,
                RX = inputMessage.RX,
                RY = inputMessage.RY,
                L2 = inputMessage.L2,
                R2 = inputMessage.R2,
            });
        }

        public void Close()
        {
            device.Closed -= disconnectedEventHandler;
            device.Close();
        }
    }
}
