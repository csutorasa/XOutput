namespace XOutput.Devices.Input
{
    public abstract class InputSource
    {
        public string DisplayName => name;
        public InputSourceTypes Type => type;
        public IDevice InputDevice => inputDevice;
        public int Offset => offset;
        public bool IsAxis => InputSourceTypes.Axis.HasFlag(type);
        public bool IsButton => type == InputSourceTypes.Button;

        protected IDevice inputDevice;
        protected string name;
        protected InputSourceTypes type;
        protected int offset;
        protected double value;

        protected InputSource(IDevice inputDevice, string name, InputSourceTypes type, int offset)
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

        public double GetValue(double defaultValue) {
            if (inputDevice == null) {
                return defaultValue;
            }
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

        private DisabledInputSource() : base(null, "", InputSourceTypes.Disabled, 0)
        {

        }
    }
}
