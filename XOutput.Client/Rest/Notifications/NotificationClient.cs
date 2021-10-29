using System.Collections.Generic;
using System.Threading.Tasks;

namespace XOutput.Rest.Notifications
{
    public class NotificationClient : HttpJsonClient
    {
        public NotificationClient(IHttpClientProvider clientProvider) : base(clientProvider)
        {

        }

        public Task<IEnumerable<Notification>> GetNotificationsAsync(int timeoutMillis = 1000)
        {
            return GetAsync<IEnumerable<Notification>>("api/notifications", CreateToken(timeoutMillis));
        }

        public Task AcknowledgeAsync(string id, int timeoutMillis = 1000)
        {
            return PutAsync("api/notifications/" + id + "/acknowledge", CreateToken(timeoutMillis));
        }
    }
}
