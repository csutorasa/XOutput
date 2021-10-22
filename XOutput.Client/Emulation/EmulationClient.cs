using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using XOutput.Api.Emulation;
using XOutput.Api.Help;

namespace XOutput.Client.Emulation
{
    public class EmulationClient : HttpJsonClient
    {
        public EmulationClient(IHttpClientProvider clientProvider) : base(clientProvider)
        {

        }

        public Task<Dictionary<string, EmulatorResponse>> GetEmulators(int timeoutMillis = 1000)
        {
            return GetAsync<Dictionary<string, EmulatorResponse>>("/api/emulators", GetToken(timeoutMillis));
        }

        public Task<IEnumerable<Api.Devices.DeviceInfo>> GetControllers(int timeoutMillis = 1000)
        {
            return GetAsync<IEnumerable<Api.Devices.DeviceInfo>>("/api/controllers", GetToken(timeoutMillis));
        }
    }
}
