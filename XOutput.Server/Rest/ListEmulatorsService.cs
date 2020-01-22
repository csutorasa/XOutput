using System.Linq;
using System.Net;
using XOutput.Api.Emulation;
using XOutput.Core.DependencyInjection;
using XOutput.Server.Emulation;

namespace XOutput.Server.Rest
{
    public class ListEmulatorsService : IRestHandler
    {
        private readonly EmulatorService emulatorService;

        [ResolverMethod]
        public ListEmulatorsService(EmulatorService emulatorService)
        {
            this.emulatorService = emulatorService;
        }

        public bool CanHandle(HttpListenerContext context)
        {
            return context.Request.Url.LocalPath == "/api/emulators" && context.Request.HttpMethod == "GET";
        }

        public void Handle(HttpListenerContext context)
        {
            var listEmulatorResponse = emulatorService.GetEmulators().ToDictionary(e => e.Name,
                e => new EmulatorResponse
                {
                    Installed = e.Installed,
                    SupportedDeviceTypes = e.SupportedDeviceTypes.Select(t => t.ToString()).ToList(),
                }
            );
            context.Response.OutputStream.WriteJson(listEmulatorResponse);
        }
    }
}
