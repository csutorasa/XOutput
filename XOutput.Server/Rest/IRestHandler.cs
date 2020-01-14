using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace XOutput.Server.Rest
{
    interface IRestHandler
    {
        bool CanHandle(HttpListenerContext context);
        void Handle(HttpListenerContext context);
    }
}
