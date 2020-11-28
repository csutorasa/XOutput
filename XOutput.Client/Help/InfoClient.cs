using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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

        public async Task<InfoResponse> GetInfo()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url + "/info"),
                Method = HttpMethod.Get,
            };
            return await GetResult<InfoResponse>(request);
        }
    }
}
