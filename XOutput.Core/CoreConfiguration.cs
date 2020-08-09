using XOutput.Core.Configuration;
using XOutput.Core.DependencyInjection;
using XOutput.Core.External;
using XOutput.Core.Versioning;
using XOutput.Core.WebSocket;

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

        [ResolverMethod]
        public static WebSocketHelper GetWebSocketHelper()
        {
            return new WebSocketHelper();
        }

        [ResolverMethod]
        public static RegistryModifierService GetRegistryModifierService()
        {
            return new RegistryModifierService();
        }

        [ResolverMethod]
        public static IVersionGetter GetIVersionGetter()
        {
            return new GithubVersionGetter();
        }

        [ResolverMethod]
        public static UpdateChecker GetUpdateChecker(IVersionGetter versionGetter)
        {
            return new UpdateChecker(versionGetter);
        }
    }
}
