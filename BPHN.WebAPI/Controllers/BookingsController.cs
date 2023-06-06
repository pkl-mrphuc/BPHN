using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    [ApiAuthorize]
    public class BookingsController : BaseController
    {
        private readonly IBookingService _bookingService;
        public BookingsController(IServiceProvider provider)
        {
            _bookingService = provider.GetRequiredService<IBookingService>();
        }

        [HttpGet]
        [Route("get-instance")]
        public IActionResult GetInstance(string id)
        {
            return Ok(_bookingService.GetInstance(id));
        }
    }
}
