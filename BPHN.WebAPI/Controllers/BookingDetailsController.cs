using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer.Attributes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

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
            Log.Debug($"BookingDetail/Cancel start: {JsonConvert.DeserializeObject(id)}");
            return Ok(await _bookingDetailService.Cancel(id));
        }

        [HttpGet]
        [Route("{date}")]
        public async Task<IActionResult> GetByDate(string date)
        {
            Log.Debug($"BookingDetail/GetByDate start: {date}");
            return Ok(await _bookingDetailService.GetByDate(date));
        }
    }
}
