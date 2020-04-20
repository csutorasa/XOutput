namespace XOutput.Devices.Input
{
    public abstract class InputSource
    {
        public string DisplayName => name;
        public SourceTypes Type => type;
        public IInputDevice InputDevice => inputDevice;
        public int Offset => offset;
        public bool IsAxis => SourceTypes.Axis.HasFlag(type);
        public bool IsButton => type == SourceTypes.Button;
        public bool IsSlider => type == SourceTypes.Slider;
        public bool IsDPad => type == SourceTypes.Dpad;

        protected IInputDevice inputDevice;
        protected string name;
        protected SourceTypes type;
        protected int offset;
        protected double value;

        protected InputSource(IInputDevice inputDevice, string name, SourceTypes type, int offset)
        {
            this.inputDevice = inputDevice;
            this.name = name;
            this.type = type;
            this.offset = offset;
        }

        public override string ToString()
        {
            return name;
        }

        public double GetValue() {
            return value;
        }

        protected bool RefreshValue(double newValue)
        {
            if (newValue != value)
            {
                value = newValue;
                return true;
            }
            return false;
        }
    }

    public class DisabledInputSource : InputSource
    {
        public static InputSource Instance => instance;
        private static InputSource instance = new DisabledInputSource();

        private DisabledInputSource() : base(null, "", SourceTypes.None, 0)
        {

        }
    }
}
