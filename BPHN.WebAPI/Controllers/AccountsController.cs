using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using BPHN.ModelLayer.Requests;
using BPHN.ModelLayer.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    public class AccountsController : BaseController
	{
		private readonly IAccountService _accountService;
		public AccountsController(IServiceProvider provider) : base(provider)
		{
			_accountService = provider.GetRequiredService<IAccountService>();
		}

		[AllowAnonymous]
		[Route("login")]
		[HttpPost]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			return Ok(await _accountService.Login(_mapper.Map<Account>(request)));
		}

		[Permission(FunctionTypeEnum.ADDUSER)]
		[ApiAuthorize]
		[Route("register")]
		[HttpPost]
		public async Task<IActionResult> RegisterForTenant([FromBody] InsertAccountRequest request)
		{
			return Ok(await _accountService.RegisterForTenant(_mapper.Map<Account>(request)));
		}

        [Permission(FunctionTypeEnum.EDITUSER)]
        [ApiAuthorize]
        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] UpdateAccountRequest request)
        {
            return Ok(await _accountService.Update(_mapper.Map<Account>(request)));
        }

        [ApiAuthorize]
		[Route("get-instance")]
		[HttpGet]
		public async Task<IActionResult> GetInstance(string id)
		{
			return Ok(await _accountService.GetInstance(id));
		}

		[AllowAnonymous]
		[Route("send-reset-password")]
		[HttpGet]
		public async Task<IActionResult> ResetPassword(string userName)
		{
			return Ok(await _accountService.ResetPassword(userName));
		}

		[AllowAnonymous]
		[Route("submit-set-password")]
		[HttpPost]
		public async Task<IActionResult> SubmitSetPassword([FromBody] SetPasswordVm request)
		{
			return Ok(await _accountService.SubmitSetPassword(request.Code, request.Password, request.UserName));
		}

		[ApiAuthorize]
		[Route("change-password")]
		[HttpPost]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
		{
			return Ok(await _accountService.ChangePassword(_mapper.Map<Account>(request)));
		}

		[Permission(FunctionTypeEnum.VIEWLISTUSER)]
		[ApiAuthorize]
		[HttpGet]
		[Route("paging")]
		public async Task<IActionResult> GetPaging(int pageIndex, int pageSize, string txtSearch)
		{
			return Ok(await _accountService.GetPaging(pageIndex, pageSize, txtSearch));
		}

		[Permission(FunctionTypeEnum.VIEWLISTUSER)]
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

		[ApiAuthorize]
		[HttpGet]
		[Route("refresh")]
		public async Task<IActionResult> Refresh()
		{
			return Ok(await _accountService.Refresh());
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("refresh-token")]
		public IActionResult RefreshToken(string refreshToken)
		{
			return Ok(_accountService.RefreshToken(refreshToken));
		}

		[HttpGet("redirect-google-login")]
		public IActionResult Login()
		{
			var redirectUrl = Url.Action("LoginGoogle", "Accounts");
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
			return Challenge(properties, GoogleDefaults.AuthenticationScheme);
		}

		[HttpGet("login-google")]
		public async Task<IActionResult> LoginGoogle()
		{
			var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return Ok(await _accountService.LoginGoogle(authenticateResult));
		}
	}
}
