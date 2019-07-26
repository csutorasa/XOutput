using System;

namespace XOutput.Tools
{
    /// <summary>
    /// General helper class.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Gets if the two doubles values are equal.
        /// </summary>
        /// <param name="a">first number</param>
        /// <param name="b">second number</param>
        /// <returns>Equality</returns>
        public static bool DoubleEquals(double a, double b)
        {
            return Math.Abs(a - b) < 0.0001;
        }
    }
}
