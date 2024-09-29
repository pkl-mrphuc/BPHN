using AutoMapper;
using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace BPHN.WebAPI.Controllers
{
    public class BookingsController : BaseController
    {
        private readonly IBookingService _bookingService;
        public BookingsController(IServiceProvider provider) : base(provider)
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

        [Permission(FunctionTypeEnum.ADDBOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("insert")]
        public async Task<IActionResult> Insert([FromBody] InsertBookingRequest request)
        {
            Log.Debug($"Booking/Insert start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _bookingService.Insert(_mapper.Map<Booking>(request)));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("insert-booking-request")]
        public async Task<IActionResult> InsertBookingRequest([FromBody] BookingRequest request)
        {
            Log.Debug($"Booking/InsertBookingRequest start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _bookingService.InsertBookingRequest(request));
        }

        [Permission(FunctionTypeEnum.ADDBOOKING, FunctionTypeEnum.EDITBOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("check-time-frame")]
        public async Task<IActionResult> CheckFreeTimeFrame([FromBody] CheckFreeTimeFrameRequest request)
        {
            Log.Debug($"Booking/CheckFreeTimeFrame start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _bookingService.CheckFreeTimeFrame(_mapper.Map<Booking>(request)));
        }

        [Permission(FunctionTypeEnum.VIEWLISTBOOKING)]
        [ApiAuthorize]
        [HttpGet]
        [Route("paging")]
        public async Task<IActionResult> GetPaging(int pageIndex, int pageSize, string txtSearch, bool hasBookingDetail = false)
        {
            Log.Debug($"Booking/GetPaging start: {JsonConvert.SerializeObject(new { PageIndex = pageIndex, PageSize = pageSize, TxtSearch = txtSearch, HasBookingDetail = hasBookingDetail })}");
            return Ok(await _bookingService.GetPagingV1(pageIndex, pageSize, txtSearch, hasBookingDetail));
        }

        [Permission(FunctionTypeEnum.VIEWLISTBOOKING)]
        [ApiAuthorize]
        [HttpGet]
        [Route("count-paging")]
        public async Task<IActionResult> GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            Log.Debug($"Booking/GetCountPaging start: {JsonConvert.SerializeObject(new { PageIndex = pageIndex, PageSize = pageSize, TxtSearch = txtSearch })}");
            return Ok(await _bookingService.GetCountPagingV1(pageIndex, pageSize, txtSearch));
        }

        [Permission(FunctionTypeEnum.ADDBOOKING, FunctionTypeEnum.EDITBOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("find-blank")]
        public async Task<IActionResult> FindBlank([FromBody] FindBlankRequeset request)
        {
            Log.Debug($"Booking/FindBlank start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _bookingService.FindBlank(_mapper.Map<Booking>(request)));
        }

        [Permission(FunctionTypeEnum.EDITBOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("approval/{id}")]
        public async Task<IActionResult> Accept(string id)
        {
            Log.Debug($"Booking/Accept start: {id}");
            return Ok(await _bookingService.Update(id, BookingStatusEnum.SUCCESS));
        }

        [Permission(FunctionTypeEnum.EDITBOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("decline/{id}")]
        public async Task<IActionResult> Decline(string id)
        {
            Log.Debug($"Booking/Decline start: {id}");
            return Ok(await _bookingService.Update(id, BookingStatusEnum.CANCEL));
        }
    }
}
