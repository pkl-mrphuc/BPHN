using BPHN.BusinessLayer.ImpServices;
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
        public IActionResult Insert([FromBody] Pitch pitch)
        {
            return Ok(_pitchService.Insert(pitch));
        }

        [ApiAuthorize]
        [Route("update")]
        [HttpPost]
        public IActionResult Update([FromBody] Pitch pitch)
        {
            return Ok(_pitchService.Update(pitch));
        }

        [ApiAuthorize]
        [Route("get-instance")]
        [HttpGet]
        public IActionResult GetInstance(string id)
        {
            return Ok(_pitchService.GetInstance(id));
        }

        [AllowAnonymous]
        [Route("paging")]
        [HttpGet]
        public IActionResult GetPaging(int pageIndex, int pageSize, string txtSearch, string accountId)
        {
            return Ok(_pitchService.GetPaging(pageIndex, pageSize, txtSearch, accountId));
        }

        [AllowAnonymous]
        [Route("count-paging")]
        [HttpGet]
        public IActionResult GetCountPaging(int pageIndex, int pageSize, string txtSearch, string accountId)
        {
            return Ok(_pitchService.GetCountPaging(pageIndex, pageSize, txtSearch, accountId));
        }
    }
}
