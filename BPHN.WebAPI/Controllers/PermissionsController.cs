using AutoMapper;
using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using BPHN.ModelLayer.Requests;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace BPHN.WebAPI.Controllers
{
    [ApiAuthorize]
    public class PermissionsController : BaseController
    {
        private readonly IPermissionService _permissionService;
        private readonly IMapper _mapper;
        public PermissionsController(IServiceProvider provider)
        {
            _permissionService = provider.GetRequiredService<IPermissionService>();
            _mapper = provider.GetRequiredService<IMapper>();
        }

        [HttpGet]
        [Route("{accountId}")]
        public async Task<IActionResult> GetPermissions(Guid accountId)
        {
            Log.Debug($"Permission/GetPermissions start: {accountId}");
            return Ok(await _permissionService.GetPermissions(accountId));
        }

        [Permission(FunctionTypeEnum.ADDUSER, FunctionTypeEnum.EDITUSER)]
        [HttpPost]
        [Route("save/{accountId}")]
        public async Task<IActionResult> SavePermissions(Guid accountId, [FromBody] List<SavePermissionRequest> request)
        {
            Log.Debug($"Permission/SavePermissions start: {JsonConvert.SerializeObject(new { AccountId = accountId, Permission = request })}");
            return Ok(await _permissionService.SavePermissions(accountId, _mapper.Map<List<Permission>>(request)));
        }
    }
}
