using System;
using XOutput.Common;

namespace XOutput.Emulation.Xbox
{
    public abstract class XboxDevice : IDevice
    {

        public string Id { get; } = Guid.NewGuid().ToString();
        public DeviceTypes DeviceType => DeviceTypes.MicrosoftXbox360;
        public abstract Emulators Emulator { get; }

        public event XboxFeedbackEvent FeedbackEvent;
        public event DeviceDisconnectedEventHandler Closed;

        protected void InvokeFeedbackEvent(XboxFeedbackEventArgs args)
        {
            FeedbackEvent?.Invoke(this, args);
        }

        protected void InvokeClosedEvent(DeviceDisconnectedEventArgs args)
        {
            Closed?.Invoke(this, args);
        }
        public abstract void SendInput(XboxInput input);
        public abstract void Close();
    }
}
