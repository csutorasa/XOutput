namespace XOutput.Number
{
    public static class NumberHelper
    {
        public static bool DoubleEquals(this double a, double b, double acceptDifference = 0.00001)
        {
            return System.Math.Abs(a - b) < acceptDifference;
        }
    }
}
