using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer.Attributes;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(await _notificationService.GetTopFiveNewNotifications());
        }
    }
}
