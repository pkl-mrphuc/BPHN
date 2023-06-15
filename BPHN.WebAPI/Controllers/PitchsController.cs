using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    public class PitchsController : BaseController
    {
        private readonly IPitchService _pitchService;
        public PitchsController(IServiceProvider provider)
        {
            _pitchService = provider.GetRequiredService<IPitchService>();
        }

        [ApiAuthorize]
        [Route("insert")]
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] Pitch pitch)
        {
            return Ok(await _pitchService.Insert(pitch));
        }

        [ApiAuthorize]
        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Pitch pitch)
        {
            return Ok(await _pitchService.Update(pitch));
        }

        [ApiAuthorize]
        [Route("get-instance")]
        [HttpGet]
        public async Task<IActionResult> GetInstance(string id)
        {
            return Ok(await _pitchService.GetInstance(id));
        }

        [AllowAnonymous]
        [Route("paging")]
        [HttpGet]
        public async Task<IActionResult> GetPaging(int pageIndex, int pageSize, string txtSearch, string accountId, bool hasDetail = false, bool hasInactive = true)
        {
            return Ok(await _pitchService.GetPaging(pageIndex, pageSize, txtSearch, accountId, hasDetail, hasInactive));
        }

        [AllowAnonymous]
        [Route("count-paging")]
        [HttpGet]
        public async Task<IActionResult> GetCountPaging(int pageIndex, int pageSize, string txtSearch, string accountId, bool hasInactive = true)
        {
            return Ok(await _pitchService.GetCountPaging(pageIndex, pageSize, txtSearch, accountId, hasInactive));
        }
    }
}
