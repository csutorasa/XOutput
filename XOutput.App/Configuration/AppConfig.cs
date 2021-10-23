using System;
using XOutput.Configuration;

namespace XOutput.App.Configuration
{
    [ConfigurationPath("conf/app")]
    public class AppConfig : ConfigurationBase, IEquatable<AppConfig>
    {
        public const string Path = "conf/app";

        public bool Minimized { get; set; }
        public string Language { get; set; }
        public string ServerUrl { get; set; }
        public bool AutoConnect { get; set; }

        public AppConfig(string language)
        {
            Minimized = false;
            Language = language;
            ServerUrl = null;
            AutoConnect = false;
        }

        public bool Equals(AppConfig other)
        {
            return Equals(Minimized, other.Minimized) &&
                Equals(Language, other.Language);
        }
    }
}
