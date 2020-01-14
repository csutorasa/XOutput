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
