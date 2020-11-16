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

        InputSource source;

        public MapperData()
        {
            InputType = "0";
            source = DisabledInputSource.Instance;
            MinValue = 0;
            MaxValue = 0;
            Deadzone = 0;
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
                var readvalue = value;
                if (Math.Abs(value - 0.5) < Deadzone)
                {
                    readvalue = 0.5;
                }

                mappedValue = (readvalue - MinValue) / range;
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
