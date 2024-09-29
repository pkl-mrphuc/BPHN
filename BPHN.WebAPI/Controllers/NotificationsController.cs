using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Serilog;

namespace BPHN.WebAPI.Controllers
{
    public class NotificationsController : BaseController
    {
        private readonly INotificationService _notificationService;
        public NotificationsController(IServiceProvider provider) : base(provider)
        {
            _notificationService = provider.GetRequiredService<INotificationService>();
        }

        [ApiAuthorize]
        [HttpGet]
        [Route("top-5")]
        public async Task<IActionResult> GetNotifications()
        {
            Log.Debug($"Notification/GetNotifications start");
            return Ok(await _notificationService.GetTopFiveNewNotifications());
        }
    }
}
