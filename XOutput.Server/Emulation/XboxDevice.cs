using XOutput.Api.Devices;
using XOutput.Api.Message.Xbox;

namespace XOutput.Server.Emulation
{
    public abstract class XboxDevice : IDevice
    {
        public DeviceTypes DeviceType => DeviceTypes.MicrosoftXbox360;

        public event XboxFeedbackEvent FeedbackEvent;
        protected void InvokeFeedbackEvent(XboxFeedbackEventArgs args)
        {
            FeedbackEvent?.Invoke(this, args);
        }
        public abstract void SendInput(XboxInputMessage input);
        public abstract void Close();
    }
}
