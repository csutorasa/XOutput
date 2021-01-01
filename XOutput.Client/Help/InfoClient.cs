using System;
using System.Net.Http;
using System.Threading.Tasks;
using XOutput.Api.Help;

namespace XOutput.Client.Help
{
    public class InfoClient : HttpJsonClient
    {
        private readonly string url;

        public InfoClient(string apiUrl)
        {
            url = apiUrl;
        }

        public Task<InfoResponse> GetInfoAsync()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url + "/info"),
                Method = HttpMethod.Get,
            };
            return GetResultAsync<InfoResponse>(request);
        }
    }
}
