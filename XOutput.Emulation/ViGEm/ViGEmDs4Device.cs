using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using XOutput.Common;
using XOutput.Emulation.Ds4;

namespace XOutput.Emulation.ViGEm
{
    public sealed class ViGEmDs4Device : Ds4Device
    {
        private readonly IDualShock4Controller controller;
        public bool Connected { get; private set; }
        public override Emulators Emulator => Emulators.ViGEm;

        public ViGEmDs4Device(IDualShock4Controller controller)
        {
            this.controller = controller;
            controller.AutoSubmitReport = false;
            controller.Connect();
            controller.FeedbackReceived += FeedbackReceived;
            Connected = true;
            SendInput(new Ds4Input
            {
                LX = 0.5,
                LY = 0.5,
                RX = 0.5,
                RY = 0.5,
            });
        }

        private void FeedbackReceived(object sender, DualShock4FeedbackReceivedEventArgs e)
        {
            InvokeFeedbackEvent(new Ds4FeedbackEventArgs
            {
                Small = (double)e.SmallMotor / byte.MaxValue,
                Large = (double)e.LargeMotor / byte.MaxValue,
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

        public override void SendInput(Ds4Input input)
        {
            SetValueIfNeeded(DualShock4Button.Circle, input.Circle);
            SetValueIfNeeded(DualShock4Button.Cross, input.Cross);
            SetValueIfNeeded(DualShock4Button.Triangle, input.Triangle);
            SetValueIfNeeded(DualShock4Button.Square, input.Square);
            SetValueIfNeeded(DualShock4Button.ShoulderLeft, input.L1);
            SetValueIfNeeded(DualShock4Button.ShoulderRight, input.R1);
            SetValueIfNeeded(DualShock4Button.ThumbLeft, input.L3);
            SetValueIfNeeded(DualShock4Button.ThumbRight, input.R3);
            SetValueIfNeeded(DualShock4Button.Share, input.Share);
            SetValueIfNeeded(DualShock4Button.Options, input.Options);
            SetValueIfNeeded(DualShock4SpecialButton.Ps, input.Ps);
            SetValueIfNeeded(DualShock4Axis.LeftThumbX, input.LX);
            SetValueIfNeeded(DualShock4Axis.LeftThumbY, input.LY);
            SetValueIfNeeded(DualShock4Axis.RightThumbX, input.RX);
            SetValueIfNeeded(DualShock4Axis.RightThumbY, input.RY);
            SetValueIfNeeded(DualShock4Slider.LeftTrigger, input.L2);
            SetValueIfNeeded(DualShock4Slider.RightTrigger, input.R2);
            SetDPadIfNeeded(input.Up, input.Down, input.Left, input.Right);
            controller.SubmitReport();
        }

        private void SetValueIfNeeded(DualShock4Button button, bool? value)
        {
            if (value.HasValue)
            {
                var newValue = value.Value;
                controller.SetButtonState(button, newValue);
            }
        }

        private void SetValueIfNeeded(DualShock4Axis axis, double? value)
        {
            if (value.HasValue)
            {
                var newValue = (byte)(value.Value * byte.MaxValue);
                controller.SetAxisValue(axis, newValue);
            }
        }

        private void SetValueIfNeeded(DualShock4Slider slider, double? value)
        {
            if (value.HasValue)
            {
                var newValue = (byte)(value.Value * byte.MaxValue);
                controller.SetSliderValue(slider, newValue);
            }
        }

        private void SetDPadIfNeeded(bool? up, bool? down, bool? left, bool? right)
        {
            if (up.HasValue || down.HasValue || left.HasValue || right.HasValue)
            {
                var newValue = GetDirection(up, down, left, right);
                controller.SetDPadDirection(newValue);
            }
        }

        private DualShock4DPadDirection GetDirection(bool? up, bool? down, bool? left, bool? right)
        {
            if (IsTrue(up))
            {
                if (IsTrue(left))
                {
                    return DualShock4DPadDirection.Northwest;
                }
                else if (IsTrue(right))
                {
                    return DualShock4DPadDirection.Northeast;
                }
                else
                {
                    return DualShock4DPadDirection.North;
                }
            }
            else if (IsTrue(down))
            {
                if (IsTrue(left))
                {
                    return DualShock4DPadDirection.Southwest;
                }
                else if (IsTrue(right))
                {
                    return DualShock4DPadDirection.Southeast;
                }
                else
                {
                    return DualShock4DPadDirection.South;
                }
            }
            else
            {
                if (IsTrue(left))
                {
                    return DualShock4DPadDirection.West;
                }
                else if (IsTrue(right))
                {
                    return DualShock4DPadDirection.East;
                }
                else
                {
                    return DualShock4DPadDirection.None;
                }
            }
        }

        private bool IsTrue(bool? b)
        {
            return b.HasValue && b.Value == true;
        }
    }
}
