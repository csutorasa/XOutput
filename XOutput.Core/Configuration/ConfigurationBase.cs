using System;
using System.Text.Json.Serialization;

namespace XOutput.Core.Configuration
{
    public class ConfigurationBase
    {
        [JsonIgnore]
        public string FilePath { get; internal set; }
    }

    public static class ConfigurationHelper
    {
        public static bool AreSame<T>(this T a, T b) where T : ConfigurationBase, IEquatable<T>
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
