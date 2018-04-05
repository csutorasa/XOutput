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
        private const string GithubURL = "https://api.github.com/repos/csutorasa/XOutput/releases/latest";

        private static readonly ILogger logger = LoggerFactory.GetLogger(typeof(UpdateChecker));
        private readonly HttpClient client = new HttpClient();

        public UpdateChecker()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            client.DefaultRequestHeaders.Add("User-Agent", "System.Net.Http.HttpClient");
        }

        private string GetLatestRelease(string response)
        {
            string tagName = Regex.Match(response, "\"tag_name\":\".*?\"").Value;
            string tags = tagName.Replace("\"tag_name\":\"", "");
            return tags.Remove(tags.Length - 1);
        }

        public async Task<VersionCompare> CompareRelease()
        {
            VersionCompare compare;
            try
            {
                logger.Debug("Getting " + GithubURL);
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

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
