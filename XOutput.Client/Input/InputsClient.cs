using System;
using System.Net.Http;
using System.Threading.Tasks;
using XOutput.Api.Help;

namespace XOutput.Client.Input
{
    public class InputsClient : HttpJsonClient
    {
        public InputsClient(IHttpClientProvider clientProvider) : base(clientProvider)
        {

        }
    }
}
