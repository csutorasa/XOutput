using System;

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
        public double Deadzone { get; set; }

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

        public void Update(InputSourceConfig config) {
            Deadzone = config.Deadzone;
        }

        public double GetValue()
        {
            return value;
        }

        protected bool RefreshValue(double newValue)
        {
            double calculatedValue = CalculatedValue(newValue);
            if (calculatedValue != value)
            {
                value = calculatedValue;
                return true;
            }
            return false;
        }

        private double CalculatedValue(double newValue)
        {
            switch (type) {
                case SourceTypes.Button:
                case SourceTypes.Dpad:
                    return newValue;
                case SourceTypes.Slider:
                    if (newValue < Deadzone) {
                        return 0;
                    }
                    if (newValue > 1 - Deadzone) {
                        return 1;
                    }
                    return newValue;
                case SourceTypes.AxisX:
                case SourceTypes.AxisY:
                case SourceTypes.AxisZ:
                case SourceTypes.Axis:
                    if (Math.Abs(newValue - 0.5) < Deadzone) {
                        return 0.5;
                    }
                    return newValue;
                default:
                    return newValue;
            }
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
