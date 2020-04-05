using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using XOutput.Api.Serialization;
using XOutput.Core.DependencyInjection;
using XOutput.Server;

namespace XOutput.App
{
    public static class AppConfiguration
    {
        [ResolverMethod]
        public static IHost GetHost()
        {
            return Host.CreateDefaultBuilder(new string[0])
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://*:8000");
                    webBuilder.UseStartup<Startup>();
                }).Build();
        }
    }
}
