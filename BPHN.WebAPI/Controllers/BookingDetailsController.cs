using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    [ApiAuthorize]
    public class BookingDetailsController : BaseController
    {
        private readonly IBookingDetailService _bookingDetailService;
        public BookingDetailsController(IServiceProvider provider)
        {
            _bookingDetailService = provider.GetRequiredService<IBookingDetailService>();
        }

        [HttpPost]
        [Route("cancel/{id}")]
        public async Task<IActionResult> Cancel(string id)
        {
            return Ok(await _bookingDetailService.Cancel(id));
        }

        [HttpGet]
        [Route("{date}")]
        public async Task<IActionResult> GetByDate(string date)
        {
            return Ok(await _bookingDetailService.GetByDate(date));
        }
    }
}
