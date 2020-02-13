using System;
using System.Collections.Generic;
using XOutput.Core.Configuration;

namespace XOutput.Server.Configuration
{
    public sealed class ServerSettings : IConfiguration, IEquatable<ServerSettings>
    {
        public List<string> Urls { get; set; }

        public ServerSettings()
        {
            Urls = new List<string>();
        }

        public bool Equals(ServerSettings other)
        {
            if (other == null) {
                return false;
            }
            return Urls.Equals(other.Urls);
        }
    }
}
