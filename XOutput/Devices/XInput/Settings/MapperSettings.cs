using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Devices.Input;

namespace XOutput.Devices.XInput.Settings
{
    public class MapperSettings
    {
        public string DeviceName { get; set; }
        public string Type { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        [JsonIgnore]
        public IInputDevice Device { get; set; }
        [JsonIgnore]
        public Enum InputType { get; set; }

        /// <summary>
        /// Gets the value based on minimum and maximum values.
        /// </summary>
        /// <param name="value">Measured data to convert</param>
        /// <returns>Mapped value</returns>
        public double GetValue(double value)
        {
            double range = MaxValue - MinValue;
            if (Math.Abs(range) < 0.0001)
                return MinValue;
            var readvalue = value;
            var mappedValue = (readvalue - MinValue) / range;
            if (mappedValue < 0)
                mappedValue = 0;
            else if (mappedValue > 1)
                mappedValue = 1;
            return mappedValue;
        }
    }
}
