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
using XOutput.Tools;

namespace XOutput.Server
{
    public class FileService
    {
        private readonly XOutputManager xOutputManager;

        private string errorPage;
        private string indexPage;

        [ResolverMethod]
        public FileService(XOutputManager xOutputManager)
        {
            this.xOutputManager = xOutputManager;

            errorPage = ReadResource("error.html");
            indexPage = ReadResource("index.html");
        }

        public bool Handle(HttpListenerContext httpContext)
        {
            if (httpContext.Request.IsWebSocketRequest)
            {
                return false;
            }
            string path = httpContext.Request.Url.LocalPath;
            if (path == "/" || path == "/index.html")
            {
                httpContext.Response.ContentType = "text/html";
                if (xOutputManager.HasDevice)
                {
#if DEBUG
                    if(File.Exists("index.html"))
                    {
                        indexPage = File.ReadAllText("index.html");
                    }
#endif
                    string customIndexPage = indexPage
                        .Replace("<<<host>>>", httpContext.Request.Url.Host)
                        .Replace("<<<port>>>", httpContext.Request.Url.Port.ToString());
                    WriteTo(httpContext.Response.OutputStream, customIndexPage);
                }
                else
                {
                    WriteTo(httpContext.Response.OutputStream, errorPage);
                }
                httpContext.Response.Close();
                return true;
            }
            return false;
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
