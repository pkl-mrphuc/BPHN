using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using BPHN.ModelLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    public class AccountsController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountsController(IServiceProvider provider)
        {
            _accountService = provider.GetRequiredService<IAccountService>();
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Account request)
        {
            return Ok(await _accountService.Login(request));
        }

        [ApiAuthorize]
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> RegisterForTenant([FromBody] Account request)
        {
            return Ok(await _accountService.RegisterForTenant(request));
        }

        [AllowAnonymous]
        [Route("send-reset-password")]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userName)
        {
            return Ok(await _accountService.ResetPassword(userName));
        }

        [AllowAnonymous]
        [Route("submit-reset-password")]
        [HttpPost]
        public async Task<IActionResult> SubmitResetPassword([FromBody] ResetPasswordVm request)
        {
            return Ok(await _accountService.SubmitResetPassword(request.Code, request.Password, request.UserName));
        }

        [ApiAuthorize]
        [Route("change-password")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] Account request)
        {
            return Ok(await _accountService.ChangePassword(request));
        }

        [ApiAuthorize]
        [HttpGet]
        [Route("paging")]
        public async Task<IActionResult> GetPaging(int pageIndex, int pageSize, string txtSearch)
        {
            return Ok(await _accountService.GetPaging(pageIndex, pageSize, txtSearch));
        }

        [ApiAuthorize]
        [HttpGet]
        [Route("count-paging")]
        public async Task<IActionResult> GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            return Ok(await _accountService.GetCountPaging(pageIndex, pageSize, txtSearch));
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("validate-token")]
        public IActionResult ValidateToken(string token)
        {
            return Ok(_accountService.ValidateToken(token));
        }
    }
}
