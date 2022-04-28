using Newtonsoft.Json;
using System;

namespace XOutput.Devices.Mapper
{
    /// <summary>
    /// Contains mapping data to Xinput conversion.
    /// </summary>
    public class MapperData
    {
        /// <summary>
        /// From data device
        /// </summary>
        public string InputDevice { get; set; }
        /// <summary>
        /// From data type
        /// </summary>
        public string InputType { get; set; }
        /// <summary>
        /// Data source
        /// </summary>
        [JsonIgnore]
        public InputSource Source
        {
            get => source;
            set
            {
                var newValue = value ?? DisabledInputSource.Instance;
                if (newValue != source)
                {
                    source = newValue;
                    InputType = source.Offset.ToString();
                    InputDevice = source.InputDevice?.UniqueId;
                }
            }
        }
        /// <summary>
        /// Minimum value
        /// </summary>
        public double MinValue { get; set; }
        /// <summary>
        /// Maximum value
        /// </summary>
        public double MaxValue { get; set; }
        /// <summary>
        /// Deadzone
        /// </summary>
        public double Deadzone { get; set; }
        /// <summary>
        /// Anti-Deadzone
        /// </summary>
        public double AntiDeadzone { get; set; }

        InputSource source;

        public MapperData()
        {
            InputType = "0";
            source = DisabledInputSource.Instance;
            MinValue = 0;
            MaxValue = 0;
            Deadzone = 0;
            AntiDeadzone = 0;
        }

        /// <summary>
        /// Gets the value based on minimum and maximum values.
        /// </summary>
        /// <param name="value">Measured data to convert</param>
        /// <returns>Mapped value</returns>
        public double GetValue(double value)
        {
            double range = MaxValue - MinValue;
            double mappedValue;
            if (Math.Abs(range) < 0.0001)
            {
                mappedValue = MinValue;
            }
            else
            {
                var readValue = value;
                
                if (Math.Abs(readValue - 0.5) < Deadzone)
                {
                    readValue = 0.5;
                }

                if (AntiDeadzone != 0)
                {
                    var sign = readValue < 0.5 ? -1 : 1;
                    readValue = (Math.Abs((readValue - 0.5) * 2) * (1 - AntiDeadzone) + AntiDeadzone) * sign / 2 + 0.5;
                }

                mappedValue = (readValue - MinValue) / range;

                if (mappedValue < 0)
                {
                    mappedValue = 0;
                }
                else if (mappedValue > 1)
                {
                    mappedValue = 1;
                }
            }
            return mappedValue;
        }
        public void SetSourceWithoutSaving(InputSource value) 
        {
            var newValue = value ?? DisabledInputSource.Instance;
            if (newValue != source)
            {
                source = newValue;
            }
        }
    }
}
