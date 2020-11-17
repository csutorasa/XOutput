using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;
using XOutput.Server.Configuration;
using XOutput.Server.Emulation;
using XOutput.Server.Input;
using XOutput.Server.Notifications;
using XOutput.Server.Websocket;

namespace XOutput.Server
{
    public class Server
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private const string ConfigurationFilepath = "conf/server";
        private readonly ServerConfig config;

        [ResolverMethod]
        public Server(ConfigurationManager configurationManager) {
            config = configurationManager.Load(ConfigurationFilepath, () => new ServerConfig {
                Urls = new List<string> { "*:8000", "localhost:8000" },
            });
            logger.Info($"Server config loaded with urls: {string.Join(", ", config.Urls)}");
        }

        public void Run() {
            using (var host = Host.CreateDefaultBuilder(new string[0])
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(config.Urls.Select(url => "http://" + url).ToArray());
                    webBuilder.UseStartup<Startup>();
                })
                .Build()) {
                host.Run();
            }
        }
    }
}
