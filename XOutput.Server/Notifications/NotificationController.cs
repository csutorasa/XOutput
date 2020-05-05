using Microsoft.AspNetCore.Mvc;
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
            return notificationService.GetAll().Select(Notification.Create).ToList();
        }
    }
}