namespace XOutput.Server.Emulation
{
    public delegate void Ds4FeedbackEvent(object sender, Ds4FeedbackEventArgs args);

    public class Ds4FeedbackEventArgs
    {
        public double Small { get; set; }
        public double Large { get; set; }
    }
}
