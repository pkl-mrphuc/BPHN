﻿using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    public class BookingDetailsController : BaseController
    {
        private readonly IBookingDetailService _bookingDetailService;
        public BookingDetailsController(IServiceProvider provider) : base(provider)
        {
            _bookingDetailService = provider.GetRequiredService<IBookingDetailService>();
        }

        [Permission(FunctionTypeEnum.EDITBOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("cancel/{id}")]
        public async Task<IActionResult> Cancel(string id)
        {
            return Ok(await _bookingDetailService.Cancel(id));
        }

        [Permission(FunctionTypeEnum.VIEWLISTBOOKINGDETAIL)]
        [ApiAuthorize]
        [HttpGet]
        [Route("{date}")]
        public async Task<IActionResult> GetByDate(string date)
        {
            return Ok(await _bookingDetailService.GetByDate(date));
        }

        [Permission(FunctionTypeEnum.VIEWLISTBOOKINGDETAIL)]
        [ApiAuthorize]
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> GetByRangeDate([FromBody] GetCalendarEventRequest request)
        {
            return Ok(await _bookingDetailService.GetByRangeDate(request));
        }

        [Permission(FunctionTypeEnum.EDITBOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("update-match")]
        public async Task<IActionResult> UpdateMatch([FromBody]CalendarEventRequest request)
        {
            return Ok(await _bookingDetailService.UpdateMatch(_mapper.Map<CalendarEvent>(request)));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetByRangeDate(string startDate, string endDate, string pitchId, string nameDetail)
        {
            return Ok(await _bookingDetailService.GetByRangeDate(startDate, endDate, pitchId, nameDetail));
        }
    }
}
