using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XOutput.UpdateChecker
{
    public class UpdateChecker
    {
        protected const string GITHUB_URL = "https://api.github.com/repos/csutorasa/XOutput/releases/latest";

        protected readonly HttpClient client = new HttpClient();

        public UpdateChecker()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            client.DefaultRequestHeaders.Add("User-Agent", "System.Net.Http.HttpClient");
        }

        protected string GetLatestRelease(string response)
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
                var response = await client.GetAsync(new Uri(GITHUB_URL));
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
    }
}
