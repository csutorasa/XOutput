using System;

namespace XOutput.Devices.Mapper
{
    public class InputMapper : Mapper
    {
        public double Deadzone { get; set; }
        public double AntiDeadzone { get; set; }
        public bool HasStaticValue { get; private set; }
        private double minValue;
        public double MinValue
        {
            get => minValue;
            set
            {
                if (minValue != value)
                {
                    minValue = value;
                    CalculateRange();
                }
            }
        }
        private double maxValue;
        public double MaxValue
        {
            get => maxValue;
            set
            {
                if (maxValue != value)
                {
                    maxValue = value;
                    CalculateRange();
                }
            }
        }

        private double range;

        public InputMapper()
        {
            MinValue = 0;
            MaxValue = 0;
            Deadzone = 0;
            AntiDeadzone = 0;
            CalculateRange();
        }

        private void CalculateRange()
        {
            range = MaxValue - MinValue;
            HasStaticValue = Math.Abs(range) < 0.0001;
        }

        public double GetValue(double value)
        {
            if (HasStaticValue)
            {
                return MinValue;
            }
            var readvalue = value;
            if (Math.Abs(value - 0.5) < Deadzone)
            {
                readvalue = 0.5;
            }
            else if (value < AntiDeadzone)
            {
                readvalue = 0;
            }
            else if (value > 1 - AntiDeadzone)
            {
                readvalue = 1;
            }

            double mappedValue = (readvalue - MinValue) / range;
            if (mappedValue < 0)
            {
                return 0;
            }
            else if (mappedValue > 1)
            {
                return 1;
            }
            return mappedValue;
        }
    }
}
