using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    public class LocationsController : BaseController
    {
        private readonly ILocationService _locationService;
        public LocationsController(IServiceProvider provider) : base(provider)
        {
            _locationService = provider.GetRequiredService<ILocationService>();
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> GetLocations([FromBody] GetLocationRequest request)
        {
            return Ok(await _locationService.GetLocations(request.TxtSearch, request.ProvinceId, request.DistrictId, request.WardId));
        }
    }
}
