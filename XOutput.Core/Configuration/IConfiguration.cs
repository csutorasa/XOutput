using System;

namespace XOutput.Core.Configuration
{
    public interface IConfiguration : IEquatable<IConfiguration>
    {

    }

    public static class ConfigurationHelper
    {
        public static bool Compare<T>(this T a, T b) where T : IConfiguration
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
