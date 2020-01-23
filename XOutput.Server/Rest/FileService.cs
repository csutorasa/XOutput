using System.IO;
using System.Net;
using System.Collections.Generic;
using XOutput.Core.DependencyInjection;

namespace XOutput.Server.Rest
{
    public class FileService : IRestHandler
    {
        private static readonly Dictionary<string, string> ExtensionMapping = new Dictionary<string, string>
        {
            { ".html", "text/html" },
            { ".js", "text/javascript" },
        };

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
            context.Response.ContentType = GetContentType(path);
            context.Response.OutputStream.WriteText(content);
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

        private string GetContentType(string path)
        {
            string extension = Path.GetExtension(path);
            string contentType;
            if (ExtensionMapping.TryGetValue(extension, out contentType))
            {
                return contentType;
            }
            return "text/plain";
        }
    }
}
