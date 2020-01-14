using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XOutput.Core.DependencyInjection;
using NLog;

namespace XOutput.Server.Rest
{
    public class RestService
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly List<IRestHandler> restHandlers;

        [ResolverMethod]
        public RestService(ApplicationContext applicationContext)
        {
            restHandlers = applicationContext.ResolveAll<IRestHandler>();
        }

        public bool Handle(HttpListenerContext httpContext)
        {
            if (httpContext.Request.IsWebSocketRequest)
            {
                return false;
            }
            List<IRestHandler> acceptedHandlers = restHandlers.Where(h => h.CanHandle(httpContext)).ToList();
            if (acceptedHandlers.Count == 0)
            {
                httpContext.Response.StatusCode = 404;
            }
            else if (acceptedHandlers.Count == 1)
            {
                acceptedHandlers[0].Handle(httpContext);
            }
            else
            {
                logger.Error("Multiple handlers found for {0}", httpContext.Request.Url);
                httpContext.Response.StatusCode = 500;
            }
            httpContext.Response.Close();
            return true;
        }
    }
}
