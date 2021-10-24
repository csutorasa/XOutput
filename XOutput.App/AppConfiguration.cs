using XOutput.DependencyInjection;
using XOutput.Rest;
using XOutput.Rest.Help;

namespace XOutput.App
{
    public static class AppConfiguration
    {
        [ResolverMethod]
        public static IHttpClientProvider HttpClientProvider()
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
