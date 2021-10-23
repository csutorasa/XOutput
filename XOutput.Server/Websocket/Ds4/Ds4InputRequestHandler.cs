using XOutput.Emulation;
using XOutput.Emulation.Ds4;

namespace XOutput.Websocket.Ds4
{
    class Ds4InputRequestHandler : IMessageHandler
    {
        internal readonly Ds4Device device;
        private readonly DeviceDisconnectedEvent disconnectedEventHandler;

        public Ds4InputRequestHandler(Ds4Device device, DeviceDisconnectedEvent disconnectedEventHandler)
        {
            this.device = device;
            this.disconnectedEventHandler = disconnectedEventHandler;
        }

        public bool CanHandle(MessageBase message)
        {
            return message is Ds4InputRequest;
        }

        public void Handle(MessageBase message)
        {
            var inputMessage = message as Ds4InputRequest;
            device.SendInput(new Ds4Input
            {
                Circle = inputMessage.Circle,
                Cross = inputMessage.Cross,
                Triangle = inputMessage.Triangle,
                Square = inputMessage.Square,
                L1 = inputMessage.L1,
                L3 = inputMessage.L3,
                R1 = inputMessage.R1,
                R3 = inputMessage.R3,
                Options = inputMessage.Options,
                Share = inputMessage.Share,
                Ps = inputMessage.Ps,
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
