using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace BPHN.WebAPI.Controllers
{
    public class BookingsController : BaseController
    {
        private readonly IBookingService _bookingService;
        public BookingsController(IServiceProvider provider)
        {
            _bookingService = provider.GetRequiredService<IBookingService>();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("get-instance")]
        public async Task<IActionResult> GetInstance(string id)
        {
            Log.Debug($"Booking/GetInstance start: {id}");
            return Ok(await _bookingService.GetInstance(id));
        }

        [Permission(FunctionTypeEnum.ADD_BOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("insert")]
        public async Task<IActionResult> Insert([FromBody] Booking request)
        {
            Log.Debug($"Booking/Insert start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _bookingService.Insert(request));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("insert-booking-request")]
        public async Task<IActionResult> InsertBookingRequest([FromBody] Booking request)
        {
            Log.Debug($"Booking/BookingRequest start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _bookingService.InsertBookingRequest(request));
        }

        [Permission(FunctionTypeEnum.EDIT_BOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("check-time-frame")]
        public async Task<IActionResult> CheckFreeTimeFrame([FromBody] Booking request)
        {
            Log.Debug($"Booking/CheckFreeTimeFrame start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _bookingService.CheckFreeTimeFrame(request));
        }

        [Permission(FunctionTypeEnum.VIEW_LIST_BOOKING)]
        [ApiAuthorize]
        [HttpGet]
        [Route("paging")]
        public async Task<IActionResult> GetPaging(int pageIndex, int pageSize, string txtSearch, bool hasBookingDetail = false)
        {
            Log.Debug($"Booking/GetPaging start: {JsonConvert.SerializeObject(new { PageIndex = pageIndex, PageSize = pageSize, TxtSearch = txtSearch, HasBookingDetail = hasBookingDetail })}");
            return Ok(await _bookingService.GetPaging(pageIndex, pageSize, txtSearch, hasBookingDetail));
        }

        [Permission(FunctionTypeEnum.VIEW_LIST_BOOKING)]
        [ApiAuthorize]
        [HttpGet]
        [Route("count-paging")]
        public async Task<IActionResult> GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            Log.Debug($"Booking/GetCountPaging start: {JsonConvert.SerializeObject(new { PageIndex = pageIndex, PageSize = pageSize, TxtSearch = txtSearch })}");
            return Ok(await _bookingService.GetCountPaging(pageIndex, pageSize, txtSearch));
        }

        [Permission(FunctionTypeEnum.ADD_PITCH)]
        [ApiAuthorize]
        [HttpPost]
        [Route("find-blank")]
        public async Task<IActionResult> FindBlank([FromBody] Booking request)
        {
            Log.Debug($"Booking/FindBlank start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _bookingService.FindBlank(request));
        }
    }
}
