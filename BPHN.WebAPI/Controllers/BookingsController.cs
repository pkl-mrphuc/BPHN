using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            return Ok(await _bookingService.GetInstance(id));
        }

        [Permission(FunctionTypeEnum.ADDBOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("insert")]
        public async Task<IActionResult> Insert([FromBody] InsertBookingRequest request)
        {
            return Ok(await _bookingService.Insert(_mapper.Map<Booking>(request)));
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("insert-booking-request")]
        public async Task<IActionResult> InsertBookingRequest([FromBody] BookingRequest request)
        {
            return Ok(await _bookingService.InsertBookingRequest(request));
        }

        [Permission(FunctionTypeEnum.ADDBOOKING, FunctionTypeEnum.EDITBOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("check-time-frame")]
        public async Task<IActionResult> CheckFreeTimeFrame([FromBody] CheckFreeTimeFrameRequest request)
        {
            return Ok(await _bookingService.CheckFreeTimeFrame(_mapper.Map<Booking>(request)));
        }

        [Permission(FunctionTypeEnum.VIEWLISTBOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("paging")]
        public async Task<IActionResult> GetPaging([FromBody] GetBookingPagingRequest request)
        {
            return Ok(await _bookingService.GetPaging(_mapper.Map<GetBookingPagingModel>(request)));
        }

        [Permission(FunctionTypeEnum.VIEWLISTBOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("count-paging")]
        public async Task<IActionResult> GetCountPaging([FromBody] GetBookingPagingRequest request)
        {
            return Ok(await _bookingService.GetCountPaging(_mapper.Map<GetBookingPagingModel>(request)));
        }

        [Permission(FunctionTypeEnum.ADDBOOKING, FunctionTypeEnum.EDITBOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("find-blank")]
        public async Task<IActionResult> FindBlank([FromBody] FindBlankRequeset request)
        {
            return Ok(await _bookingService.FindBlank(_mapper.Map<Booking>(request)));
        }

        [Permission(FunctionTypeEnum.EDITBOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("approval/{id}")]
        public async Task<IActionResult> Accept(string id)
        {
            return Ok(await _bookingService.Update(id, BookingStatusEnum.SUCCESS));
        }

        [Permission(FunctionTypeEnum.EDITBOOKING)]
        [ApiAuthorize]
        [HttpPost]
        [Route("decline/{id}")]
        public async Task<IActionResult> Decline(string id)
        {
            return Ok(await _bookingService.Update(id, BookingStatusEnum.CANCEL));
        }
    }
}
