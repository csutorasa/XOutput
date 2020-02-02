using XOutput.Core.DependencyInjection;

namespace XOutput.Devices.XInput
{
    public class XOutputManager
    {

        public bool HasDevice => true;

        public bool IsVigem => true;

        public bool IsScp => true;

        private readonly ApplicationContext applicationContext;


        [ResolverMethod]
        public XOutputManager(ApplicationContext applicationContext)
        {
            this.applicationContext = applicationContext;
        }

        public WebsocketXboxClient Start()
        {
            var client = applicationContext.Resolve<WebsocketXboxClient>();
            client.Start();
            return client;
        }

        public void Stop(WebsocketXboxClient client)
        {
            client.Stop();
        }
    }
}
