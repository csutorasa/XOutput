using System;

namespace XOutput.Core.Configuration
{
    public interface IConfiguration
    {

    }

    public static class ConfigurationHelper
    {
        public static bool AreSame<T>(this T a, T b) where T : IConfiguration, IEquatable<T>
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }
            if (a == null || b == null)
            {
                return false;
            }
            if (a.GetType() == b.GetType())
            {
                return a.Equals(b);
            }
            return false;
        }
    }
}
