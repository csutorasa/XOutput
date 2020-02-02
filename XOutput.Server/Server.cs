using System;
using System.Threading;
using XOutput.Core;
using XOutput.Core.DependencyInjection;
using XOutput.Server.Http;

namespace XOutput.Server
{
    public class Server
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly HttpServer server;

        public static void Main()
        {
            var server = new Server();
            server.WaitForExit();
            server.Close();
        }

        public Server()
        {
            Console.CancelKeyPress += CancelKeyPress;
            var globalContext = ApplicationContext.Global;
            globalContext.Discover();
            globalContext.AddFromConfiguration(typeof(CoreConfiguration));
            globalContext.AddFromConfiguration(typeof(ApiConfiguration));
            /*var firewallService = globalContext.Resolve<FirewallService>();
            firewallService.AddException();*/
            server = globalContext.Resolve<HttpServer>();
            server.Start("http://192.168.1.2:8000/");
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
            server.Stop();
        }
    }
}
