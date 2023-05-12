﻿using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
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

        [Route("reset-password")]
        [HttpPost]
        public IActionResult ResetPassword(string userName)
        {
            return Ok(_accountService.ResetPassword(userName));
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
    }
}
