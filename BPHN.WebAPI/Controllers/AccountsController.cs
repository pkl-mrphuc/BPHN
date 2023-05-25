using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using BPHN.ModelLayer.Others;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public IActionResult Login([FromBody] Account request)
        {
            return Ok(_accountService.Login(request));
        }

        [ApiAuthorize]
        [Route("register")]
        [HttpPost]
        public IActionResult RegisterForTenant([FromBody] Account request)
        {
            return Ok(_accountService.RegisterForTenant(request));
        }

        [AllowAnonymous]
        [Route("send-reset-password")]
        [HttpGet]
        public IActionResult ResetPassword(string userName)
        {
            return Ok(_accountService.ResetPassword(userName));
        }

        [AllowAnonymous]
        [Route("submit-reset-password/{password}")]
        [HttpPost]
        public IActionResult SubmitResetPassword([FromBody] string code, string password)
        {
            return Ok(_accountService.SubmitResetPassword(code, password));
        }

        [ApiAuthorize]
        [HttpGet]
        [Route("paging")]
        public IActionResult GetPaging(int pageIndex, int pageSize, string txtSearch)
        {
            return Ok(_accountService.GetPaging(pageIndex, pageSize, txtSearch));
        }

        [ApiAuthorize]
        [HttpGet]
        [Route("count-paging")]
        public IActionResult GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            return Ok(_accountService.GetCountPaging(pageIndex, pageSize, txtSearch));
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
