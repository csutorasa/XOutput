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
using XOutput.Devices.XInput;
using XOutput.Core.DependencyInjection;

namespace XOutput.Server
{
    public class FileService
    {
        private readonly XOutputManager xOutputManager;

        private string errorPage;

        [ResolverMethod]
        public FileService(XOutputManager xOutputManager)
        {
            this.xOutputManager = xOutputManager;

            errorPage = ReadResource("error.html");
        }

        public bool Handle(HttpListenerContext httpContext)
        {
            if (httpContext.Request.IsWebSocketRequest)
            {
                return false;
            }
            string path = httpContext.Request.Url.LocalPath;
            if (xOutputManager.HasDevice)
            {
                string file = path;
                if (file == "/")
                {
                    file = "/index.html";
                }
                file = "." + file.Replace("/", "\\");
                if(!File.Exists(file))
                {
                    return false;
                }
                var content = File.ReadAllText(file);
                string customContent = content
                    .Replace("<<<host>>>", httpContext.Request.Url.Host)
                    .Replace("<<<port>>>", httpContext.Request.Url.Port.ToString());
                httpContext.Response.ContentType = "text/html";
                WriteTo(httpContext.Response.OutputStream, customContent);
            }
            else
            {
                httpContext.Response.ContentType = "text/html";
                WriteTo(httpContext.Response.OutputStream, errorPage);
            }
            httpContext.Response.Close();
            return true;
        }

        private string ReadResource(string resource)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Assembly.GetExecutingAssembly().GetName().Name + ".Resources.Web." + resource))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Encoding.UTF8.GetString(assemblyData);
            }
        }

        private void WriteTo(Stream outputStream, string text)
        {
            using(StreamWriter sw = new StreamWriter(outputStream))
            {
                sw.Write(text);
            }
        }
    }
}
