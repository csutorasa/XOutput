using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Core.DependencyInjection;
using XOutput.Core.External;
using XOutput.Server.Rest;
using XOutput.Server.Websocket;

namespace XOutput.Server.Http
{
    public class HttpServer
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly CommandRunner commandRunner;
        private readonly RestService restService;
        private readonly WebSocketService webSocketService;

        private bool running;
        private CancellationTokenSource cancellationTokenSource;
        private HttpListener listener;

        [ResolverMethod(Scope.Prototype)]
        public HttpServer(CommandRunner commandRunner, RestService restService, WebSocketService webSocketService)
        {
            this.commandRunner = commandRunner;
            this.restService = restService;
            this.webSocketService = webSocketService;
        }

        public void Configure(List<string> uris)
        {
            if (running)
            {
                return;
            }
            if (listener == null)
            {
                listener = new HttpListener();
                uris.ForEach(uri => listener.Prefixes.Add(uri));
            }
            else
            {
                listener.Prefixes.Clear();
                uris.ForEach(uri => listener.Prefixes.Add(uri));
            }
        }

        public void Start()
        {
            if (running)
            {
                return;
            }
            cancellationTokenSource = new CancellationTokenSource();
            try
            {
                listener.Start();
            }
            catch (HttpListenerException ex)
            {
                logger.Warn(ex);
                List<string> uris = listener.Prefixes.ToList();
                listener = null;
                AddPermissions(uris);
                Configure(uris);
                listener.Start();
            }
            running = true;
            Task.Run(() => AcceptClientsAsync(listener));
        }

        public void AddPermissions(List<string> uris)
        {
            var domainUser = Environment.UserDomainName + "\\\\" + Environment.UserName;
            string uri = string.Join(",", uris);
            var process = commandRunner.CreatePowershell($"netsh http add urlacl url={uri} user={domainUser}");
            commandRunner.RunProcess(process);
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
                catch (Exception e)
                {
                    logger.Error(e, "Failed to stop http server");
                }
            }
        }

        private async Task AcceptClientsAsync(HttpListener server)
        {
            while (running)
            {
                try
                {
                    var httpContext = await server.GetContextAsync();
                    if (!webSocketService.Handle(httpContext, cancellationTokenSource.Token) && !restService.Handle(httpContext))
                    {
                        httpContext.Response.StatusCode = 404;
                        httpContext.Response.Close();
                    }
                }
                catch (Exception ex)
                {
                    if (server.IsListening)
                    {
                        logger.Error(ex, "Failed to handle connection");
                    }
                }
            }
        }
    }
}
