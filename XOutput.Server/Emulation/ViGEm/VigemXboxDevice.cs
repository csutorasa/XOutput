using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using XOutput.Api.Devices;
using XOutput.Api.Message.Xbox;

namespace XOutput.Server.Emulation.ViGEm
{
    public sealed class ViGEmXboxDevice : XboxDevice
    {
        private readonly IXbox360Controller controller;

        public ViGEmXboxDevice(IXbox360Controller controller)
        {
            this.controller = controller;
            controller.AutoSubmitReport = false;
            controller.Connect();
            SetValueIfNeeded(Xbox360Axis.LeftThumbX, 0.5);
            SetValueIfNeeded(Xbox360Axis.LeftThumbY, 0.5);
            SetValueIfNeeded(Xbox360Axis.RightThumbX, 0.5);
            SetValueIfNeeded(Xbox360Axis.RightThumbY, 0.5);
            controller.SubmitReport();
        }

        public override void Close()
        {
            controller.Disconnect();
        }

        public override void SendInput(XboxInputMessage input)
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
