using System;
using System.Collections.Generic;
using System.Linq;
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
            return await GetResult<InfoResponse>(new Uri(url + "/info"));
        }
    }
}
