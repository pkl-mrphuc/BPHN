using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using BPHN.ModelLayer.Others;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace BPHN.WebAPI.Controllers
{
    public class BookingDetailsController : BaseController
    {
        private readonly IBookingDetailService _bookingDetailService;
        public BookingDetailsController(IServiceProvider provider)
        {
            _bookingDetailService = provider.GetRequiredService<IBookingDetailService>();
        }

        [Permission(new[] { FunctionTypeEnum.EDIT_BOOKING })]
        [ApiAuthorize]
        [HttpPost]
        [Route("cancel/{id}")]
        public async Task<IActionResult> Cancel(string id)
        {
            Log.Debug($"BookingDetail/Cancel start: {id}");
            return Ok(await _bookingDetailService.Cancel(id));
        }

        [Permission(new[] { FunctionTypeEnum.VIEW_LIST_BOOKING_DETAIL })]
        [ApiAuthorize]
        [HttpGet]
        [Route("{date}")]
        public async Task<IActionResult> GetByDate(string date)
        {
            Log.Debug($"BookingDetail/GetByDate start: {date}");
            return Ok(await _bookingDetailService.GetByDate(date));
        }

        [Permission(new[] { FunctionTypeEnum.EDIT_BOOKING })]
        [ApiAuthorize]
        [HttpPost]
        [Route("update-match")]
        public async Task<IActionResult> UpdateMatch([FromBody]CalendarEvent request)
        {
            Log.Debug($"BookingDetail/UpdateMatch start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _bookingDetailService.UpdateMatch(request));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetByRangeDate(string startDate, string endDate, string pitchId, string nameDetail)
        {
            Log.Debug($"BookingDetail/GetByRangeDate start: {JsonConvert.SerializeObject(new { StartDate = startDate, EndDate = endDate, PitchId = pitchId, NameDetail = nameDetail })}");
            return Ok(await _bookingDetailService.GetByRangeDate(startDate, endDate, pitchId, nameDetail));
        }
    }
}
