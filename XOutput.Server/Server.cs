using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
        private const string ConfigurationFilepath = "conf/server";
        private readonly ConfigurationManager configurationManager;
        private readonly ServerConfig config;

        private IHost host;

        [ResolverMethod]
        public Server(ConfigurationManager configurationManager) {
            this.configurationManager = configurationManager;
            this.config = configurationManager.Load(ConfigurationFilepath, () => new ServerConfig {
                Urls = new List<string> { "*:8000" },
            });
            this.host = Host.CreateDefaultBuilder(new string[0])
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(config.Urls.Select(url => "http://" + url).ToArray());
                    webBuilder.UseStartup<Startup>();
                }).Build();
        }

        public IHost GetHost()
        {
            return host;
        }
    }
}
