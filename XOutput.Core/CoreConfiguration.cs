using XOutput.Configuration;
using XOutput.DependencyInjection;
using XOutput.External;
using XOutput.Versioning;
using XOutput.Websocket;

namespace XOutput
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
