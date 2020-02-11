using System.Collections.Generic;
using XOutput.Core.Configuration;

namespace XOutput.Server.Configuration
{
    public class ServerSettings : IConfiguration
    {
        public List<string> Urls { get; set; }

        public ServerSettings()
        {
            Urls = new List<string>();
        }

        public bool Equals(IConfiguration settings)
        {
            var other = settings as ServerSettings;
            return Urls.Equals(other.Urls);
        }
    }
}
