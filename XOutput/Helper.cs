using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput
{
    public static class Helper
    {
        public static bool DoubleEquals(double a, double b)
        {
            return Math.Abs(a - b) < 0.0001;
        }
    }
}
