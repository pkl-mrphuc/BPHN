using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
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

        [HttpPost]
        [Route("insert")]
        public IActionResult Insert([FromBody] Booking request)
        {
            return Ok(_bookingService.Insert(request));
        }

        [HttpPost]
        [Route("check-time-frame")]
        public IActionResult CheckFreeTimeFrame([FromBody] Booking request)
        {
            return Ok(_bookingService.CheckFreeTimeFrame(request));
        }

        [HttpGet]
        [Route("paging")]
        public IActionResult GetPaging(int pageIndex, int pageSize, string txtSearch, bool hasBookingDetail = false)
        {
            return Ok(_bookingService.GetPaging(pageIndex, pageSize, txtSearch, hasBookingDetail));
        }

        [HttpGet]
        [Route("count-paging")]
        public IActionResult GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            return Ok(_bookingService.GetCountPaging(pageIndex, pageSize, txtSearch));
        }
    }
}
