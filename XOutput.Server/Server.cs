using System;
using System.IO;
using System.Reflection;
using System.Threading;
using NLog;
using XOutput.Core;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;
using XOutput.Server.Configuration;
using XOutput.Server.Http;

namespace XOutput.Server
{
    public class Server
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        private static string settingsPath;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private HttpServer server;
        private IDisposable fileWatcher;

        public static void Main()
        {
            string exePath = Assembly.GetExecutingAssembly().Location;
            string cwd = Path.GetDirectoryName(exePath);
            Directory.SetCurrentDirectory(cwd);
            settingsPath = Path.Combine(cwd, "config", "server.json");

            var main = new Server();
            main.WaitForExit();
            main.Close();
        }

        public Server()
        {

            Console.CancelKeyPress += CancelKeyPress;
            var globalContext = ApplicationContext.Global;
            globalContext.Discover();
            globalContext.AddFromConfiguration(typeof(CoreConfiguration));
            globalContext.AddFromConfiguration(typeof(ApiConfiguration));
            var configurationManager = globalContext.Resolve<ConfigurationManager>();
            /*var firewallService = globalContext.Resolve<FirewallService>();
            firewallService.AddException();*/
            RestartServer(globalContext, configurationManager);
            fileWatcher = configurationManager.Watch(settingsPath, (path) => {
                RestartServer(globalContext, configurationManager);
            });
        }

        private void RestartServer(ApplicationContext context, ConfigurationManager configurationManager)
        {
            try
            {
                var settings = configurationManager.Load(settingsPath, () => new ServerSettings());
                var newServer = context.Resolve<HttpServer>();
                newServer.Configure(settings.Urls);
                if (server != null) {
                    server.Stop();
                }
                server = newServer;
                server.Start();
            }
            catch (Exception e)
            {
                logger.Error(e, "Invalid server configuration");
            }
        }

        private void CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            cancellationTokenSource.Cancel();
            Console.CancelKeyPress -= CancelKeyPress;
        }

        private void WaitForExit()
        {
            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }
        }

        private void Close()
        {
            fileWatcher?.Dispose();
            server?.Stop();
        }
    }
}
