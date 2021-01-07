using System;
using System.Net.Http;
using System.Threading.Tasks;
using XOutput.Api.Help;

namespace XOutput.Client.Help
{
    public class InfoClient : HttpJsonClient
    {
        public InfoClient(Uri baseUri) : this(new HttpClient { BaseAddress = baseUri })
        {

        }

        public InfoClient(HttpClient client) : base(client)
        {

        }

        public Task<InfoResponse> GetInfoAsync(int timeoutMillis = 1000)
        {
            return GetAsync<InfoResponse>("info", GetToken(timeoutMillis));
        }
    }
}
