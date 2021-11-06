using System.Collections.Generic;
using System.Threading.Tasks;

namespace XOutput.Rest.Emulation
{
    public class EmulationClient : HttpJsonClient
    {
        public EmulationClient(IHttpClientProvider clientProvider) : base(clientProvider)
        {

        }

        public Task<Dictionary<string, EmulatorResponse>> GetEmulators(int timeoutMillis = 1000)
        {
            return GetAsync<Dictionary<string, EmulatorResponse>>("/api/emulators", CreateToken(timeoutMillis));
        }
    }
}
