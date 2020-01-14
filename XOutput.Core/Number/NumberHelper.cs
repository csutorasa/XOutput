using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Core.Number
{
    public static class NumberHelper
    {
        public static bool DoubleEquals(this double a, double b, double acceptDifference = 0.00001)
        {
            return System.Math.Abs(a - b) < acceptDifference;
        }
    }
}
