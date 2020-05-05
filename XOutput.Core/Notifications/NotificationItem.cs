using System;
using System.Collections.Generic;
using System.Text;

namespace XOutput.Core.Notifications
{
    public class NotificationItem
    {
        public string Key { get; set; }
        public NotificationTypes NotificationType { get; set; }
        public List<string> Parameters { get; set; }
        public DateTime Timeout { get; set; }
    }
}
