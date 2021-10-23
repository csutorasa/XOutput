using XOutput.Emulation;
using XOutput.Emulation.Xbox;

namespace XOutput.Websocket.Xbox
{
    class XboxInputRequestHandler : IMessageHandler
    {
        internal readonly XboxDevice device;
        private readonly DeviceDisconnectedEvent disconnectedEventHandler;

        public XboxInputRequestHandler(XboxDevice device, DeviceDisconnectedEvent disconnectedEventHandler)
        {
            this.device = device;
            this.disconnectedEventHandler = disconnectedEventHandler;
        }

        public bool CanHandle(MessageBase message)
        {
            return message is XboxInputRequest;
        }

        public void Handle(MessageBase message)
        {
            var inputMessage = message as XboxInputRequest;
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
                Up = inputMessage.UP,
                Down = inputMessage.DOWN,
                Left = inputMessage.LEFT,
                Right = inputMessage.RIGHT,
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
