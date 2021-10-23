using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Configuration;
using XOutput.DependencyInjection;

namespace XOutput
{
    public class HttpServer
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly ServerConfig config;

        [ResolverMethod]
        public HttpServer(ConfigurationManager configurationManager) {
            config = configurationManager.Load(() => new ServerConfig {
                Urls = new List<string> { "*:8000", "localhost:8000" },
            });
            logger.Info($"Server config loaded with urls: {string.Join(", ", config.Urls)}");
        }

        public void Run() {
            using var host = Host.CreateDefaultBuilder(Array.Empty<string>())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(config.Urls.Select(url => "http://" + url).ToArray());
                    webBuilder.UseStartup<Startup>();
                })
                .Build();
            host.Run();
        }
    }
}
