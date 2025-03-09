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
using Newtonsoft.Json;
using Serilog;

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
			Log.Debug($"Account/Login start: {JsonConvert.SerializeObject(request)}");
			return Ok(await _accountService.Login(_mapper.Map<Account>(request)));
		}

		[Permission(FunctionTypeEnum.ADDUSER)]
		[ApiAuthorize]
		[Route("register")]
		[HttpPost]
		public async Task<IActionResult> RegisterForTenant([FromBody] InsertAccountRequest request)
		{
			Log.Debug($"Account/RegisterForTenant start: {JsonConvert.SerializeObject(request)}");
			return Ok(await _accountService.RegisterForTenant(_mapper.Map<Account>(request)));
		}

		[ApiAuthorize]
		[Route("get-instance")]
		[HttpGet]
		public async Task<IActionResult> GetInstance(string id)
		{
			Log.Debug($"Account/GetInstance start: {id}");
			return Ok(await _accountService.GetInstance(id));
		}

		[AllowAnonymous]
		[Route("send-reset-password")]
		[HttpGet]
		public async Task<IActionResult> ResetPassword(string userName)
		{
			Log.Debug($"Account/ResetPassword start: {userName}");
			return Ok(await _accountService.ResetPassword(userName));
		}

		[AllowAnonymous]
		[Route("submit-set-password")]
		[HttpPost]
		public async Task<IActionResult> SubmitSetPassword([FromBody] SetPasswordVm request)
		{
			Log.Debug($"Account/SubmitSetPassword start: {JsonConvert.SerializeObject(request)}");
			return Ok(await _accountService.SubmitSetPassword(request.Code, request.Password, request.UserName));
		}

		[ApiAuthorize]
		[Route("change-password")]
		[HttpPost]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
		{
			Log.Debug($"Account/ChangePassword start: {JsonConvert.SerializeObject(request)}");
			return Ok(await _accountService.ChangePassword(_mapper.Map<Account>(request)));
		}

		[Permission(FunctionTypeEnum.VIEWLISTUSER)]
		[ApiAuthorize]
		[HttpGet]
		[Route("paging")]
		public async Task<IActionResult> GetPaging(int pageIndex, int pageSize, string txtSearch)
		{
			Log.Debug($"Account/GetPaging start: {JsonConvert.SerializeObject(new { PageIndex = pageIndex, PageSize = pageSize, TxtSearch = txtSearch })}");
			return Ok(await _accountService.GetPaging(pageIndex, pageSize, txtSearch));
		}

		[Permission(FunctionTypeEnum.VIEWLISTUSER)]
		[ApiAuthorize]
		[HttpGet]
		[Route("count-paging")]
		public async Task<IActionResult> GetCountPaging(int pageIndex, int pageSize, string txtSearch)
		{
			Log.Debug($"Account/GetCountPaging start: {JsonConvert.SerializeObject(new { PageIndex = pageIndex, PageSize = pageSize, TxtSearch = txtSearch })}");
			return Ok(await _accountService.GetCountPaging(pageIndex, pageSize, txtSearch));
		}

		[AllowAnonymous]
		[HttpGet]
		[Route("validate-token")]
		public IActionResult ValidateToken(string token)
		{
			Log.Debug($"Account/ValidateToken start: {token}");
			return Ok(_accountService.ValidateToken(token));
		}

		[ApiAuthorize]
		[HttpGet]
		[Route("refresh")]
		public async Task<IActionResult> Refresh()
		{
			Log.Debug($"Account/Refresh start");
			return Ok(await _accountService.Refresh());
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("refresh-token")]
		public IActionResult RefreshToken(string refreshToken)
		{
			Log.Debug($"Account/RefreshToken start: {refreshToken}");
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
