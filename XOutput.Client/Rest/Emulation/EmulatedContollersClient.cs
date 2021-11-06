using System.Collections.Generic;
using System.Threading.Tasks;

namespace XOutput.Rest.Emulation
{
    public class EmulatedContollersClient : HttpJsonClient
    {
        public EmulatedContollersClient(IHttpClientProvider clientProvider) : base(clientProvider)
        {

        }

        public Task<IEnumerable<ControllerInfo>> GetControllers(int timeoutMillis = 1000)
        {
            return GetAsync<IEnumerable<ControllerInfo>>("/api/controllers", CreateToken(timeoutMillis));
        }

        public Task CreateController(CreateControllerRequest request, int timeoutMillis = 1000)
        {
            return PutAsync<CreateControllerRequest>("/api/controllers", request, CreateToken(timeoutMillis));
        }

        public Task StartController(string id, int timeoutMillis = 1000)
        {
            return PutAsync($"/api/controllers/{id}/active", CreateToken(timeoutMillis));
        }

        public Task StopController(string id, int timeoutMillis = 1000)
        {
            return DeleteAsync($"/api/controllers/{id}/active", CreateToken(timeoutMillis));
        }

        public Task DeleteController(string id, int timeoutMillis = 1000)
        {
            return DeleteAsync($"/api/controllers/{id}", CreateToken(timeoutMillis));
        }
    }
}
