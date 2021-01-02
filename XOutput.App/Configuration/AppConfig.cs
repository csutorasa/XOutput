using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Core.Configuration;

namespace XOutput.App.Configuration
{
    public class AppConfig : ConfigurationBase, IEquatable<AppConfig>
    {
        public bool Minimized { get; set; }

        public AppConfig()
        {
            Minimized = false;
        }

        public bool Equals(AppConfig other)
        {
            return Equals(Minimized, other.Minimized);
        }
    }
}
