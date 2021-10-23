using System;
using System.Collections.Generic;

namespace XOutput.Configuration
{
    [ConfigurationPath("conf/server")]
    public class ServerConfig : ConfigurationBase, IEquatable<ServerConfig>
    {
        public List<string> Urls { get; set; }

        public ServerConfig()
        {
            Urls = new List<string>();
        }

        public bool Equals(ServerConfig other)
        {
            return Equals(Urls, other.Urls);
        }
    }
}