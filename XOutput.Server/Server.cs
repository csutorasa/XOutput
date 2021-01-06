using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;
using XOutput.Server.Configuration;

namespace XOutput.Server
{
    public class Server
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private readonly ServerConfig config;

        [ResolverMethod]
        public Server(ConfigurationManager configurationManager) {
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
