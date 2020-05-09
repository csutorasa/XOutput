using System;

namespace XOutput.Emulation.Ds4
{
    public abstract class Ds4Device : IDevice
    {

        public string Id { get; } = Guid.NewGuid().ToString();
        public DeviceTypes DeviceType => DeviceTypes.SonyDualShock4;

        public event Ds4FeedbackEvent FeedbackEvent;
        public event DeviceDisconnectedEvent Closed;

        protected void InvokeFeedbackEvent(Ds4FeedbackEventArgs args)
        {
            FeedbackEvent?.Invoke(this, args);
        }

        protected void InvokeClosedEvent(DeviceDisconnectedEventArgs args)
        {
            Closed?.Invoke(this, args);
        }
        public abstract void SendInput(Ds4Input input);
        public abstract void Close();
    }
}
