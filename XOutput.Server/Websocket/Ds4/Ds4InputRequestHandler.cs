using XOutput.Emulation;
using XOutput.Emulation.Ds4;

namespace XOutput.Websocket.Ds4
{
    class Ds4InputRequestHandler : MessageHandler
    {
        private readonly Ds4Device device;
        private readonly EmulatedControllersService emulatedControllersService;
        private readonly DeviceDisconnectedEventHandler disconnectedEventHandler;

        public Ds4InputRequestHandler(CloseFunction closeFunction, SenderFunction senderFunction, Ds4Device device,
         EmulatedControllersService emulatedControllersService, DeviceDisconnectedEventHandler disconnectedEventHandler) : base(closeFunction, senderFunction)
        {
            this.device = device;
            this.emulatedControllersService = emulatedControllersService;
            this.disconnectedEventHandler = disconnectedEventHandler;
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

        protected override void HandleMessage(MessageBase message)
        {
            if (message is Ds4InputRequest) {
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
                    Up = inputMessage.Up,
                    Down = inputMessage.Down,
                    Left = inputMessage.Left,
                    Right = inputMessage.Right,
                    LX = inputMessage.LX,
                    LY = inputMessage.LY,
                    RX = inputMessage.RX,
                    RY = inputMessage.RY,
                    L2 = inputMessage.L2,
                    R2 = inputMessage.R2,
                });
            }
        }

        public override void Close()
        {
            base.Close();
            emulatedControllersService.StopAndRemove(device.Id);
            device.Closed -= disconnectedEventHandler;
            device.FeedbackEvent -= FeedbackEvent;
            device.Close();
        }
    }
}
