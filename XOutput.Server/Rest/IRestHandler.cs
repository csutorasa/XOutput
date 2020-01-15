using System.Net;

namespace XOutput.Server.Rest
{
    interface IRestHandler
    {
        bool CanHandle(HttpListenerContext context);
        void Handle(HttpListenerContext context);
    }
}
