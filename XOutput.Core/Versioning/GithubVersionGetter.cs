using NLog;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using XOutput.Core.DependencyInjection;

namespace XOutput.Core.Versioning
{
    public sealed class GithubVersionGetter : IVersionGetter, IDisposable
    {
        /// <summary>
        /// GitHub URL to check the latest release version.
        /// </summary>
        private const string GithubURL = "https://raw.githubusercontent.com/csutorasa/XOutput/master/latest.version";

        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly HttpClient client = new HttpClient();

        [ResolverMethod]
        public GithubVersionGetter()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            client.DefaultRequestHeaders.Add("User-Agent", "System.Net.Http.HttpClient");
        }


        /// <summary>
        /// Releases all resources.
        /// </summary>
        public void Dispose()
        {
            client.Dispose();
        }

        public async Task<string> GetLatestReleaseAsync()
        {
            HttpResponseMessage response = null;
            try
            {
                logger.Debug("Getting " + GithubURL);
                response = await client.GetAsync(new Uri(GithubURL));
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                return content.Trim();
            }
            catch (Exception e)
            {
                throw new Exception("Failed to get latest version", e);
            }
            finally
            {
                response?.Dispose();
            }
        }
    }
}
