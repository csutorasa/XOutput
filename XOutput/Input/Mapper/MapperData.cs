using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Input.Mapper
{
    /// <summary>
    /// Contains mapping data to X input conversion.
    /// </summary>
    public class MapperData
    {
        /// <summary>
        /// From data type
        /// </summary>
        public Enum InputType { get; set; }
        /// <summary>
        /// Minimum value
        /// </summary>
        public double MinValue { get; set; }
        /// <summary>
        /// Maximum value
        /// </summary>
        public double MaxValue { get; set; }

        public MapperData()
        {
            InputType = null;
            MinValue = 0;
            MaxValue = 1;
        }
        
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
            var mappedValue = (value - MinValue) / range;
            return mappedValue < 0 ? 0 : (mappedValue > 1 ? 1 : mappedValue);
        }
    }
}
