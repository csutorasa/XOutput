using System.Collections.Generic;
using System.Threading.Tasks;
using XOutput.Rest.Emulation;

namespace XOutput.Rest.Mapping
{
    public class MappedContollersClient : HttpJsonClient
    {
        public MappedContollersClient(IHttpClientProvider clientProvider) : base(clientProvider)
        {

        }

        public Task<IEnumerable<MappedControllerInfo>> GetControllers(int timeoutMillis = 1000)
        {
            return GetAsync<IEnumerable<MappedControllerInfo>>("/api/mapped/controllers", CreateToken(timeoutMillis));
        }

        public Task CreateController(CreateControllerRequest request, int timeoutMillis = 1000)
        {
            return PutAsync<CreateControllerRequest>("/api/mapped/controllers", request, CreateToken(timeoutMillis));
        }

        public Task StartController(string id, int timeoutMillis = 1000)
        {
            return PutAsync($"/api/mapped/controllers/{id}/active", CreateToken(timeoutMillis));
        }

        public Task StopController(string id, int timeoutMillis = 1000)
        {
            return DeleteAsync($"/api/mapped/controllers/{id}/active", CreateToken(timeoutMillis));
        }


        public Task DeleteController(string id, int timeoutMillis = 1000)
        {
            return DeleteAsync($"/api/mapped/controllers/{id}", CreateToken(timeoutMillis));
        }
    }
}
