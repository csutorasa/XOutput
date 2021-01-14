using XOutput.Api.Serialization;
using XOutput.Client;
using XOutput.Client.Help;
using XOutput.Core.DependencyInjection;

namespace XOutput.App
{
    public static class AppConfiguration
    {
        [ResolverMethod]
        public static IHttpClientProvider GetHttpClientProvider()
        {
            return new DynamicHttpClientProvider();
        }

        [ResolverMethod]
        public static InfoClient InfoClient(IHttpClientProvider httpClientProvider)
        {
            return new InfoClient(httpClientProvider);
        }
    }
}
