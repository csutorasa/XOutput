using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using XOutput.Emulation.Xbox;

namespace XOutput.Emulation.ViGEm
{
    public sealed class ViGEmXboxDevice : XboxDevice
    {
        private readonly IXbox360Controller controller;
        public bool Connected { get; private set; }

        public ViGEmXboxDevice(IXbox360Controller controller)
        {
            this.controller = controller;
            controller.AutoSubmitReport = false;
            controller.Connect();
            controller.FeedbackReceived += FeedbackReceived;
            Connected = true;
            SendInput(new XboxInput
            {
                LX = 0.5,
                LY = 0.5,
                RX = 0.5,
                RY = 0.5,
            });
        }

        private void FeedbackReceived(object sender, Xbox360FeedbackReceivedEventArgs e)
        {
            InvokeFeedbackEvent(new XboxFeedbackEventArgs
            {
                Small = (double)e.SmallMotor / byte.MaxValue,
                Large = (double)e.LargeMotor / byte.MaxValue,
                LedNumber = e.LedNumber,
            });
        }

        public override void Close()
        {
            if (Connected)
            {
                Connected = false;
                controller.FeedbackReceived -= FeedbackReceived;
                controller.Disconnect();
                InvokeClosedEvent(new DeviceDisconnectedEventArgs());
            }
        }

        public override void SendInput(XboxInput input)
        {
            SetValueIfNeeded(Xbox360Button.A, input.A);
            SetValueIfNeeded(Xbox360Button.B, input.B);
            SetValueIfNeeded(Xbox360Button.X, input.X);
            SetValueIfNeeded(Xbox360Button.Y, input.Y);
            SetValueIfNeeded(Xbox360Button.LeftShoulder, input.L1);
            SetValueIfNeeded(Xbox360Button.RightShoulder, input.R1);
            SetValueIfNeeded(Xbox360Button.LeftThumb, input.L3);
            SetValueIfNeeded(Xbox360Button.RightThumb, input.R3);
            SetValueIfNeeded(Xbox360Button.Back, input.Back);
            SetValueIfNeeded(Xbox360Button.Start, input.Start);
            SetValueIfNeeded(Xbox360Button.Guide, input.Home);
            SetValueIfNeeded(Xbox360Button.Up, input.UP);
            SetValueIfNeeded(Xbox360Button.Down, input.DOWN);
            SetValueIfNeeded(Xbox360Button.Left, input.LEFT);
            SetValueIfNeeded(Xbox360Button.Right, input.RIGHT);
            SetValueIfNeeded(Xbox360Axis.LeftThumbX, input.LX);
            SetValueIfNeeded(Xbox360Axis.LeftThumbY, input.LY);
            SetValueIfNeeded(Xbox360Axis.RightThumbX, input.RX);
            SetValueIfNeeded(Xbox360Axis.RightThumbY, input.RY);
            SetValueIfNeeded(Xbox360Slider.LeftTrigger, input.L2);
            SetValueIfNeeded(Xbox360Slider.RightTrigger, input.R2);
            controller.SubmitReport();
        }

        private void SetValueIfNeeded(Xbox360Button button, bool? value)
        {
            if (value.HasValue)
            {
                var newValue = value.Value;
                controller.SetButtonState(button, newValue);
            }
        }

        private void SetValueIfNeeded(Xbox360Axis axis, double? value)
        {
            if (value.HasValue)
            {
                var newValue = (short)((value.Value - 0.5) * 2 * short.MaxValue);
                controller.SetAxisValue(axis, newValue);
            }
        }

        private void SetValueIfNeeded(Xbox360Slider slider, double? value)
        {
            if (value.HasValue)
            {
                var newValue = (byte)(value.Value * byte.MaxValue);
                controller.SetSliderValue(slider, newValue);
            }
        }
    }
}
