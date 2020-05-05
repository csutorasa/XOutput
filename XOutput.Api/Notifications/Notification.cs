using System;
using System.Collections.Generic;
using System.Text;
using XOutput.Core.Notifications;

namespace XOutput.Api.Notifications
{
    public class Notification
    {
        public const string Information = "Info";
        public const string Warning = "Warn";
        public const string Error = "Error";

        public string Key { get; set; }
        public string Level { get; set; }
        public string[] Parameters { get; set; }

        public static Notification Create(NotificationItem item)
        {
            return new Notification
            {
                Key = item.Key,
                Level = GetLevel(item.NotificationType),
                Parameters = item.Parameters.ToArray(),
            };
        }

        private static string GetLevel(NotificationTypes type)
        {
            switch(type)
            {
                case NotificationTypes.Information:
                    return Information;
                case NotificationTypes.Warning:
                    return Warning;
                case NotificationTypes.Error:
                    return Error;
                default:
                    throw new ArgumentException(nameof(type));
            }
        }
    }
}
