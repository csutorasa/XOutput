using System.Collections.Generic;
using System.Threading.Tasks;

namespace XOutput.Rest.Emulation
{
    public class EmulatedContollersClient : HttpJsonClient
    {
        public EmulatedContollersClient(IHttpClientProvider clientProvider) : base(clientProvider)
        {

        }

        public Task<IEnumerable<EmulatedControllerInfo>> GetControllers(int timeoutMillis = 1000)
        {
            return GetAsync<IEnumerable<EmulatedControllerInfo>>("/api/emulated/controllers", CreateToken(timeoutMillis));
        }

        public Task DeleteController(string id, int timeoutMillis = 1000)
        {
            return DeleteAsync($"/api/emulated/controllers/{id}", CreateToken(timeoutMillis));
        }
    }
}
