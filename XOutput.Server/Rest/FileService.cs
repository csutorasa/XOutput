using System.IO;
using System.Net;
using XOutput.Core.DependencyInjection;

namespace XOutput.Server.Rest
{
    public class FileService : IRestHandler
    {

        [ResolverMethod]
        public FileService()
        {

        }

        public bool CanHandle(HttpListenerContext context)
        {
            string path = GetPath(context);
            return File.Exists(path);
        }

        public void Handle(HttpListenerContext context)
        {
            string path = GetPath(context);
            var content = File.ReadAllText(path);
            string customContent = content
                .Replace("<<<host>>>", context.Request.Url.Host)
                .Replace("<<<port>>>", context.Request.Url.Port.ToString());
            context.Response.ContentType = "text/html";
            context.Response.OutputStream.WriteText(customContent);
        }

        private string GetPath(HttpListenerContext context)
        {
            string path = context.Request.Url.LocalPath;
            string file = path;
            if (file == "/")
            {
                file = "/index.html";
            }
            return ".\\web" + file.Replace("/", "\\");
        }
    }
}
