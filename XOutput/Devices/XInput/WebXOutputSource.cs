namespace XOutput.Devices.XInput
{
    public class WebXOutputSource : InputSource
    {
        public XInputTypes XInputType => inputType;

        private readonly XInputTypes inputType;
        private double newValue;

        public WebXOutputSource(string name, XInputTypes type) : base(null, name, type.GetInputSourceType(), 0)
        {
            inputType = type;
            newValue = type.GetDisableValue();
        }

        internal bool Refresh()
        {
            return RefreshValue(newValue);
        }

        public void SetValue(double value)
        {
            newValue = value;
        }
    }
}
