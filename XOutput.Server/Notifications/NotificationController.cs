using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using XOutput.Api.Devices;
using XOutput.Api.Input;
using XOutput.Api.Notifications;
using XOutput.Core.DependencyInjection;
using XOutput.Core.Notifications;
using XOutput.Devices;
using XOutput.Devices.Input;
using XOutput.Server.Emulation.HidGuardian;

namespace XOutput.Server.Notifications
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
        public ActionResult<IEnumerable<Notification>> ListInputDevices()
        {
            return notificationService.GetAll().Select(Create).ToList();
        }


        public static Notification Create(NotificationItem item)
        {
            return new Notification
            {
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