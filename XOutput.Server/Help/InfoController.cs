using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Reflection;
using XOutput.Core.DependencyInjection;

namespace XOutput.Server.Help
{
    public class InfoController : Controller
    {
        private readonly string version;

        [ResolverMethod]
        public InfoController()
        {
            version = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
        }

        [HttpGet]
        [Route("/api/info")]
        public ActionResult<object> ListNotifications()
        {
            return new
            {
                Version = version,
            };
        }
    }
}