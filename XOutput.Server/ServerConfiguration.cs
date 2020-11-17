using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using XOutput.Api.Serialization;
using XOutput.Core.DependencyInjection;
using XOutput.Server;

namespace XOutput.App
{
    public static class ServerConfiguration
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
