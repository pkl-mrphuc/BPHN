﻿using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using BPHN.ModelLayer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

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
            Log.Debug($"Account/Login start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _accountService.Login(request));
        }

        [ApiAuthorize]
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> RegisterForTenant([FromBody] Account request)
        {
            Log.Debug($"Account/RegisterForTenant start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _accountService.RegisterForTenant(request));
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
        [Route("submit-reset-password")]
        [HttpPost]
        public async Task<IActionResult> SubmitResetPassword([FromBody] ResetPasswordVm request)
        {
            Log.Debug($"Account/SubmitResetPassword start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _accountService.SubmitResetPassword(request.Code, request.Password, request.UserName));
        }

        [ApiAuthorize]
        [Route("change-password")]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] Account request)
        {
            Log.Debug($"Account/ChangePassword start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _accountService.ChangePassword(request));
        }

        [ApiAuthorize]
        [HttpGet]
        [Route("paging")]
        public async Task<IActionResult> GetPaging(int pageIndex, int pageSize, string txtSearch)
        {
            Log.Debug($"Account/GetPaging start: {JsonConvert.SerializeObject(new { PageIndex = pageIndex, PageSize = pageSize, TxtSearch = txtSearch } )}");
            return Ok(await _accountService.GetPaging(pageIndex, pageSize, txtSearch));
        }

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
    }
}
