﻿using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    [ApiAuthorize]
    public class HistoryLogsController : BaseController
    {
        private readonly IHistoryLogService _historyLogService;
        public HistoryLogsController(IServiceProvider provider, IHistoryLogService historyLogService) : base(provider)
        {
            _historyLogService = historyLogService;
        }

        [HttpGet]
        [Route("paging")]
        public async Task<IActionResult> GetPaging(int pageIndex, int pageSize, string txtSearch) 
        {
            return Ok(await _historyLogService.GetPaging(pageIndex, pageSize, txtSearch));
        }

        [HttpGet]
        [Route("count-paging")]
        public async Task<IActionResult> GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            return Ok(await _historyLogService.GetCountPaging(pageIndex, pageSize, txtSearch));
        }

        [HttpGet]
        [Route("description")]
        public async Task<IActionResult> GetDescription(string historyLogId)
        {
            return Ok(await _historyLogService.GetDescription(historyLogId));
        }
    }
}
