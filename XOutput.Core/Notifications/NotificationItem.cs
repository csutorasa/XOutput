using System;
using System.Collections.Generic;

namespace XOutput.Notifications
{
    public class NotificationItem
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public bool Acknowledged { get; set; }
        public NotificationTypes NotificationType { get; set; }
        public List<string> Parameters { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Timeout { get; set; }
    }
}
