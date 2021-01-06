using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Core.Configuration;

namespace XOutput.App.Configuration
{
    [ConfigurationPath("conf/app")]
    public class AppConfig : ConfigurationBase, IEquatable<AppConfig>
    {
        public const string Path = "conf/app";

        public bool Minimized { get; set; }
        public string Language { get; set; }

        public AppConfig(string language)
        {
            Minimized = false;
            Language = language;
        }

        public bool Equals(AppConfig other)
        {
            return Equals(Minimized, other.Minimized) &&
                Equals(Language, other.Language);
        }
    }
}
