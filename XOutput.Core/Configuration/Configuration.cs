using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Core.Configuration
{
    public interface Configuration : IEquatable<Configuration>
    {

    }

    public static class ConfigurationHelper
    {
        public static bool Compare<T>(this T a, T b) where T : Configuration
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
