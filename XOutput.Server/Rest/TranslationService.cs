using System.Net;
using XOutput.Core.DependencyInjection;

namespace XOutput.Server.Rest
{
    public class TranslationService : IRestHandler
    {

        [ResolverMethod]
        public TranslationService()
        {

        }

        public bool CanHandle(HttpListenerContext context)
        {
            return false;
        }

        public void Handle(HttpListenerContext context)
        {

        }
    }
}
