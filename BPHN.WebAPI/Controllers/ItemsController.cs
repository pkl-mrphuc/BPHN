using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using BPHN.ModelLayer.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    public class ItemsController : BaseController
    {
        private readonly IItemService _itemService;
        public ItemsController(IServiceProvider provider) : base(provider)
        {
            _itemService = provider.GetRequiredService<IItemService>();
        }

        [Permission(FunctionTypeEnum.VIEWLISTSERVICE)]
        [ApiAuthorize]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetItems(string txtSearch, string status, string code, string unit, string quantity)
        {
            return Ok(await _itemService.GetItems(txtSearch, status, code, unit, quantity));
        }

        [ApiAuthorize]
        [Route("get-instance")]
        [HttpGet]
        public async Task<IActionResult> GetInstance(string id)
        {
            return Ok(await _itemService.GetInstance(id));
        }

        [Permission(FunctionTypeEnum.ADDSERVICE)]
        [ApiAuthorize]
        [HttpPost]
        [Route("insert")]
        public async Task<IActionResult> Insert([FromBody] InsertItemRequest request)
        {
            return Ok(await _itemService.Insert(_mapper.Map<Item>(request)));
        }

        [Permission(FunctionTypeEnum.EDITSERVICE)]
        [ApiAuthorize]
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdateItemRequest request)
        {
            return Ok(await _itemService.Update(_mapper.Map<Item>(request)));
        }
    }
}
