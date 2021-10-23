using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.DependencyInjection;

namespace XOutput.Notifications
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
                    Id = new Guid().ToString(),
                    Key = key,
                    Acknowledged = false,
                    NotificationType = notificationType,
                    Parameters = parameters == null ? new List<string>() : parameters.ToList(),
                    CreatedAt = DateTime.Now,
                    Timeout = timeout,
                });
                Cleanup();
            }
        }

        public bool Acknowledge(string id)
        {
            foreach (var notification in notifications.Where(n => n.Id == id))
            {
                notification.Acknowledged = true;
                return true;
            }
            return false;
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
