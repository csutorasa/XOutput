using XOutput.Emulation;
using XOutput.Emulation.Xbox;

namespace XOutput.Websocket.Xbox
{
    class XboxInputRequestHandler : MessageHandler
    {
        private readonly XboxDevice device;
        private readonly EmulatedControllersService emulatedControllersService;
        private readonly DeviceDisconnectedEventHandler disconnectedEventHandler;

        public XboxInputRequestHandler(CloseFunction closeFunction, SenderFunction senderFunction, XboxDevice device, EmulatedControllersService emulatedControllersService, DeviceDisconnectedEventHandler disconnectedEventHandler) : base(closeFunction, senderFunction)
        {
            this.device = device;
            this.emulatedControllersService = emulatedControllersService;
            this.disconnectedEventHandler = disconnectedEventHandler;
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

        protected override void HandleMessage(MessageBase message)
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
