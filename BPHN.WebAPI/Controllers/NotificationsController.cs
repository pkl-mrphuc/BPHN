using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer.Attributes;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BPHN.WebAPI.Controllers
{
    [ApiAuthorize]
    public class NotificationsController : BaseController
    {
        private readonly INotificationService _notificationService;
        public NotificationsController(IServiceProvider provider)
        {
            _notificationService = provider.GetRequiredService<INotificationService>();
        }

        [HttpGet]
        [Route("top-5")]
        public async Task<IActionResult> GetNotifications()
        {
            Log.Debug($"Notification/GetNotifications start");
            return Ok(await _notificationService.GetTopFiveNewNotifications());
        }
    }
}
