using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using BPHN.ModelLayer.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    public class PitchsController : BaseController
    {
        private readonly IPitchService _pitchService;
        public PitchsController(IServiceProvider provider) : base(provider)
        {
            _pitchService = provider.GetRequiredService<IPitchService>();
        }

        [Permission(FunctionTypeEnum.ADDPITCH)]
        [ApiAuthorize]
        [Route("insert")]
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] InsertPitchRequest request)
        {
            return Ok(await _pitchService.Insert(_mapper.Map<Pitch>(request)));
        }

        [Permission(FunctionTypeEnum.EDITPITCH)]
        [ApiAuthorize]
        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdatePitchRequest request)
        {
            return Ok(await _pitchService.Update(_mapper.Map<Pitch>(request)));
        }

        [ApiAuthorize]
        [Route("get-instance")]
        [HttpGet]
        public async Task<IActionResult> GetInstance(string id)
        {
            return Ok(await _pitchService.GetInstance(id));
        }

        [Permission(FunctionTypeEnum.VIEWLISTPITCH)]
        [ApiAuthorize]
        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetPitchs(bool onlyActive = true)
        {
            return Ok(await _pitchService.GetPitchs(onlyActive));
        }

        [Permission(FunctionTypeEnum.VIEWLISTPITCH)]
        [AllowAnonymous]
        [Route("paging")]
        [HttpGet]
        public async Task<IActionResult> GetPaging(int pageIndex, int pageSize, string txtSearch, string accountId, bool hasDetail = false, bool hasInactive = true)
        {
            return Ok(await _pitchService.GetPaging(pageIndex, pageSize, txtSearch, accountId, hasDetail, hasInactive));
        }

        [Permission(FunctionTypeEnum.VIEWLISTPITCH)]
        [AllowAnonymous]
        [Route("count-paging")]
        [HttpGet]
        public async Task<IActionResult> GetCountPaging(int pageIndex, int pageSize, string txtSearch, string accountId, bool hasInactive = true)
        {
            return Ok(await _pitchService.GetCountPaging(pageIndex, pageSize, txtSearch, accountId, hasInactive));
        }
    }
}
