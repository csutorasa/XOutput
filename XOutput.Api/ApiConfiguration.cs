using XOutput.Api.Serialization;
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
