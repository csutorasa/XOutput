using System.Collections.Generic;
using System.Threading.Tasks;

namespace XOutput.Rest.Input
{
    public class InputsClient : HttpJsonClient
    {
        public InputsClient(IHttpClientProvider clientProvider) : base(clientProvider)
        {

        }

        public Task<IEnumerable<InputDeviceInfo>> GetInputsAsync(int timeoutMillis = 1000)
        {
            return GetAsync<IEnumerable<InputDeviceInfo>>("api/inputs", CreateToken(timeoutMillis));
        }

        public Task<InputDeviceInfo> GetInputAsync(string id, int timeoutMillis = 1000)
        {
            return GetAsync<InputDeviceInfo>("api/inputs/" + id, CreateToken(timeoutMillis));
        }
    }
}
