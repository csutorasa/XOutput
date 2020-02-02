using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Exceptions;
using XOutput.Core.External;

namespace XOutput.Core
{
    public static class CoreConfiguration
    {
        [ResolverMethod]
        public static ConfigurationManager GetConfigurationManager()
        {
            return new JsonConfigurationManager();
        }

        [ResolverMethod]
        public static CommandRunner GetCommandRunner()
        {
            return new CommandRunner();
        }
    }
}
