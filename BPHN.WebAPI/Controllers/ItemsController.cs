using BPHN.BusinessLayer.ImpServices;
using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using BPHN.ModelLayer.Requests;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

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
        public async Task<IActionResult> GetItems()
        {
            Log.Debug($"Item/GetItems start:");
            return Ok(await _itemService.GetItems());
        }

        [ApiAuthorize]
        [Route("get-instance")]
        [HttpGet]
        public async Task<IActionResult> GetInstance(string id)
        {
            Log.Debug($"Item/GetInstance start: {id}");
            return Ok(await _itemService.GetInstance(id));
        }

        [Permission(FunctionTypeEnum.ADDSERVICE)]
        [ApiAuthorize]
        [HttpPost]
        [Route("insert")]
        public async Task<IActionResult> Insert([FromBody] InsertItemRequest request)
        {
            Log.Debug($"Item/Insert start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _itemService.Insert(_mapper.Map<Item>(request)));
        }

        [Permission(FunctionTypeEnum.EDITSERVICE)]
        [ApiAuthorize]
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UpdateItemRequest request)
        {
            Log.Debug($"Item/Update start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _itemService.Update(_mapper.Map<Item>(request)));
        }
    }
}
