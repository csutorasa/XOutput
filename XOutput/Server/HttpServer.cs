using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Logging;
using XOutput.Tools;
using XOutput.Core.DependencyInjection;

namespace XOutput.Server
{
    public class HttpServer : IDisposable
    {
        private static ILogger logger = LoggerFactory.GetLogger(typeof(HttpServer));

        private readonly CommandRunner commandRunner;
        private readonly FileService fileService;
        private readonly WebSocketService webSocketService;

        private bool running;
        private CancellationTokenSource cancellationTokenSource;
        private HttpListener listener;

        [ResolverMethod]
        public HttpServer(CommandRunner commandRunner, FileService fileService, WebSocketService webSocketService)
        {
            this.commandRunner = commandRunner;
            this.fileService = fileService;
            this.webSocketService = webSocketService;
        }

        public void Start(string uri)
        {
            if (running)
            {
                return;
            }
            cancellationTokenSource = new CancellationTokenSource();
            listener = new HttpListener();
            listener.Prefixes.Add(uri);
            try
            {
                listener.Start();
            } catch(HttpListenerException ex)
            {
                logger.Warning(ex);
                var domainUser = WindowsIdentity.GetCurrent().Name;
                commandRunner.RunCmdAdmin($"netsh http add urlacl url={uri} user={domainUser}");
                listener = new HttpListener();
                listener.Prefixes.Add(uri);
                listener.Start();
            }
            running = true;
            Task.Run(() => AcceptClientsAsync(listener));
        }

        public void AddPersmissions(string uri)
        {
            var domainUser = WindowsIdentity.GetCurrent().Name;
            commandRunner.RunCmd($"netsh http add urlacl url={uri} user={domainUser}");
        }

        public void Stop()
        {
            if (listener != null && running)
            {
                try
                {
                    cancellationTokenSource.Cancel();
                    running = false;
                    listener.Stop();
                    listener = null;
                }
                catch
                {
                    logger.Error("Failed to stop http server");
                }
            }
        }

        public void Dispose()
        {
            Stop();
        }

        private async Task AcceptClientsAsync(HttpListener server)
        {
            while (running)
            {
                try
                {
                    var httpContext = await server.GetContextAsync();
                    if (!webSocketService.Handle(httpContext, cancellationTokenSource.Token) && !fileService.Handle(httpContext))
                    {
                        httpContext.Response.StatusCode = 404;
                        httpContext.Response.Close();
                    }
                } 
                catch (Exception ex)
                {
                    logger.Error("Failed to handle connection", ex);
                }
            }
        }
    }
}
