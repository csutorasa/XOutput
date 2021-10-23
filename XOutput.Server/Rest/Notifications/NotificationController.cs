using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.DependencyInjection;
using XOutput.Notifications;

namespace XOutput.Rest.Notifications
{
    public class NotificationController : Controller
    {
        private readonly NotificationService notificationService;

        [ResolverMethod]
        public NotificationController(NotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [HttpGet]
        [Route("/api/notifications")]
        public ActionResult<IEnumerable<Notification>> ListNotifications()
        {
            return notificationService.GetAll().Select(Create).ToList();
        }

        [HttpPut]
        [Route("/api/notifications/{id}/acknowledge")]
        public ActionResult AcknowledgeNotification(string id)
        {
            if (notificationService.Acknowledge(id))
            {
                return NoContent();
            }
            return NotFound();
        }

        public static Notification Create(NotificationItem item)
        {
            return new Notification
            {
                Id = item.Id,
                Acknowledged = item.Acknowledged,
                CreatedAt = item.CreatedAt.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"),
                Key = item.Key,
                Level = GetLevel(item.NotificationType),
                Parameters = item.Parameters.ToList(),
            };
        }

        private static string GetLevel(NotificationTypes type)
        {
            return type switch
            {
                NotificationTypes.Information => Notification.Information,
                NotificationTypes.Warning => Notification.Warning,
                NotificationTypes.Error => Notification.Error,
                _ => throw new ArgumentException(nameof(type)),
            };
        }
    }
}