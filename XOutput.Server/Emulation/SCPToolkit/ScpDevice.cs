using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;
using XOutput.Api.Devices;
using XOutput.Api.Message.Xbox;

namespace XOutput.Server.Emulation.SCPToolkit
{
    public sealed class ScpDevice : XboxDevice
    {

        public bool Connected { get; private set; }

        private readonly ScpClient client;
        private readonly int controllerCount;

        private readonly byte[] report = new byte[20];

        public ScpDevice(int controllerCount, ScpClient client)
        {
            this.client = client;
            this.controllerCount = controllerCount;
            report[0] = 0; // Input report
            report[1] = 20; // Message length
            client.Plugin(controllerCount);
            SendInput(new XboxInputMessage
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
                Small = e.SmallMotor / byte.MaxValue,
                Large = e.LargeMotor / byte.MaxValue,
                LedNumber = e.LedNumber,
            });
        }

        public override void Close()
        {
            if (Connected)
            {
                Connected = false;
                client.Unplug(controllerCount);
            }
        }

        public override void SendInput(XboxInputMessage input)
        {
            SetValueIfNeeded(2, 0, input.UP);
            SetValueIfNeeded(2, 1, input.DOWN);
            SetValueIfNeeded(2, 2, input.LEFT);
            SetValueIfNeeded(2, 3, input.RIGHT);
            SetValueIfNeeded(2, 4, input.Start);
            SetValueIfNeeded(2, 5, input.Back);
            SetValueIfNeeded(2, 6, input.L3);
            SetValueIfNeeded(2, 7, input.R3);
            SetValueIfNeeded(3, 0, input.L1);
            SetValueIfNeeded(3, 1, input.R1);
            SetValueIfNeeded(3, 2, input.Home);
            SetValueIfNeeded(3, 4, input.A);
            SetValueIfNeeded(3, 5, input.B);
            SetValueIfNeeded(3, 6, input.X);
            SetValueIfNeeded(3, 7, input.Y);
            SetValueIfNeeded(4, input.L2);
            SetValueIfNeeded(5, input.R2);
            SetValueIfNeeded(6, 7, input.LX);
            SetValueIfNeeded(8, 9, input.LY);
            SetValueIfNeeded(10, 11, input.RX);
            SetValueIfNeeded(12, 13, input.RY);
            client.Report(controllerCount, report);
        }

        private void SetValueIfNeeded(int index, int bit, bool? set)
        {
            if (set.HasValue)
            {
                int mask = 1 << bit;
                if (set.Value)
                {
                    report[index] |= (byte)mask;
                }
                else
                {
                    report[index] &= (byte)~mask;
                }
            }
        }

        private void SetValueIfNeeded(int index, double? value)
        {
            if (value.HasValue)
            {
                report[index] = (byte)(value.Value * byte.MaxValue);
            }
        }

        private void SetValueIfNeeded(int index1, int index2, double? value)
        {
            if (value.HasValue)
            {
                ushort axisValue = (ushort)((value.Value - 0.5) * ushort.MaxValue);
                report[index1] = (byte)(axisValue & 0xFF);
                report[index2] = (byte)((axisValue >> 8) & 0xFF);
            }
        }
    }
}
