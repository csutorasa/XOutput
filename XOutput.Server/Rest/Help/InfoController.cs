using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using XOutput.DependencyInjection;

namespace XOutput.Rest.Help
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
        public ActionResult<InfoResponse> ListNotifications()
        {
            return new InfoResponse
            {
                Version = version,
            };
        }
    }
}