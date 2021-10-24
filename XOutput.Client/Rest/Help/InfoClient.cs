using System.Threading.Tasks;

namespace XOutput.Rest.Help
{
    public class InfoClient : HttpJsonClient
    {
        public InfoClient(IHttpClientProvider clientProvider) : base(clientProvider)
        {

        }

        public Task<InfoResponse> GetInfoAsync(int timeoutMillis = 1000)
        {
            return GetAsync<InfoResponse>("info", CreateToken(timeoutMillis));
        }
    }
}
