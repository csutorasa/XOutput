namespace XOutput.Devices.Input
{
    public class ForceFeedbackTarget
    {
        public string DisplayName => name;
        public IDevice InputDevice => inputDevice;
        public int Offset => offset;

        protected IDevice inputDevice;
        protected string name;
        protected int offset;

        protected ForceFeedbackTarget(IDevice inputDevice, string name, int offset)
        {
            this.inputDevice = inputDevice;
            this.name = name;
            this.offset = offset;
        }

        public override string ToString()
        {
            return name;
        }

        public void SetValue(double value) {
            // TODO force feedback
        }
    }
}
