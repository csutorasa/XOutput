using System;
using XOutput.Api.Devices;
using XOutput.Api.Message.Xbox;

namespace XOutput.Server.Emulation
{
    public abstract class XboxDevice : IDevice
    {

        public string Id { get; } = Guid.NewGuid().ToString();
        public DeviceTypes DeviceType => DeviceTypes.MicrosoftXbox360;

        public event XboxFeedbackEvent FeedbackEvent;
        public event DeviceDisconnectedEvent Closed;

        protected void InvokeFeedbackEvent(XboxFeedbackEventArgs args)
        {
            FeedbackEvent?.Invoke(this, args);
        }

        protected void InvokeClosedEvent(DeviceDisconnectedEventArgs args)
        {
            Closed?.Invoke(this, args);
        }
        public abstract void SendInput(XboxInputMessage input);
        public abstract void Close();
    }
}
