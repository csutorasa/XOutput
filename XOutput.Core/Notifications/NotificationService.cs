using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Core.DependencyInjection;

namespace XOutput.Core.Notifications
{
    public class NotificationService
    {
        private readonly List<NotificationItem> notifications = new List<NotificationItem>();
        private readonly object sync = new object();

        [ResolverMethod]
        public NotificationService()
        {

        }

        public void Add(string key, IEnumerable<string> parameters, NotificationTypes notificationType = NotificationTypes.Information)
        {
            Add(key, parameters, notificationType, DateTime.MaxValue);
        }

        public void Add(string key, IEnumerable<string> parameters, NotificationTypes notificationType, DateTime timeout)
        {
            lock (sync)
            {
                notifications.Add(new NotificationItem
                {
                    Key = key,
                    NotificationType = notificationType,
                    Parameters = parameters.ToList(),
                    Timeout = timeout,
                });
                Cleanup();
            }
        }



        public IEnumerable<NotificationItem> GetAll()
        {
            lock (sync)
            {
                Cleanup();
                return notifications.ToArray();
            }
        }

        private void Cleanup()
        {
            notifications.RemoveAll(n => n.Timeout < DateTime.Now);
        }
    }
}
