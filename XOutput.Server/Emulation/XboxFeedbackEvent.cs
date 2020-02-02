using XOutput.Api.Devices;
using XOutput.Api.Message.Xbox;

namespace XOutput.Server.Emulation
{
    public delegate void XboxFeedbackEvent(object sender, XboxFeedbackEventArgs args);

    public class XboxFeedbackEventArgs
    {
        public double Small { get; set; }
        public double Large { get; set; }
        public int LedNumber { get; set; }
    }
}
