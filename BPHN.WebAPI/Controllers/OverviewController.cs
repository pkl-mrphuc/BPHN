using BPHN.BusinessLayer.ImpServices;
using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using BPHN.ModelLayer.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    public class OverviewController : BaseController
    {
        private readonly IOverviewService _overviewService;
        public OverviewController(IServiceProvider provider) : base(provider)
        {
            _overviewService = provider.GetRequiredService<IOverviewService>();
        }

        [ApiAuthorize]
        [HttpPost("")]
        [Permission(FunctionTypeEnum.VIEWSTATISTIC)]
        public async Task<IActionResult> GetStatistics([FromBody] GetStatisticsRequest request)
        {
            return Ok(await _overviewService.GetStatistics(request));
        }
    }
}
