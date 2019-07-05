using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XOutput.Logging;

namespace XOutput.UpdateChecker
{
    public sealed class UpdateChecker : IDisposable
    {
        /// <summary>
        /// GitHub URL to check the latest release version.
        /// </summary>
        private const string GithubURL = "https://raw.githubusercontent.com/csutorasa/XOutput/master/latest.version";

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(UpdateChecker));
        private readonly HttpClient client = new HttpClient();

        public UpdateChecker()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            client.DefaultRequestHeaders.Add("User-Agent", "System.Net.Http.HttpClient");
        }

        /// <summary>
        /// Gets the string of the latest release from a http response.
        /// </summary>
        /// <param name="response">GitHub response</param>
        /// <returns></returns>
        private string GetLatestRelease(string response)
        {
            return response;
        }

        /// <summary>
        /// Compares the current version with the latest release.
        /// </summary>
        /// <returns></returns>
        public async Task<VersionCompare> CompareRelease()
        {
            VersionCompare compare;
            try
            {
                await logger.Debug("Getting " + GithubURL);
                var response = await client.GetAsync(new Uri(GithubURL));
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                string latestRelease = GetLatestRelease(content);
                compare = Version.Compare(latestRelease);
            }
            catch (Exception)
            {
                compare = VersionCompare.Error;
            }
            return await Task.Run(() => compare);
        }

        /// <summary>
        /// Releases all resources.
        /// </summary>
        public void Dispose()
        {
            client.Dispose();
        }
    }
}
