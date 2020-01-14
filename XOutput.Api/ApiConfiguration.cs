using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Api.Serialization;
using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;

namespace XOutput.Core
{
    public static class ApiConfiguration
    {
        [ResolverMethod]
        public static MessageReader GetMessageReader()
        {
            return new MessageReader();
        }

        [ResolverMethod]
        public static MessageWriter GetMessageWriter()
        {
            return new MessageWriter();
        }
    }
}
