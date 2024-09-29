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
    public class ConfigsController : BaseController
    {
        private readonly IConfigService _configService;
        public ConfigsController(IServiceProvider provider) : base(provider)
        {
            _configService = provider.GetRequiredService<IConfigService>();
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetConfigs(string key)
        {
            Log.Debug($"Config/GetConfigs start: {key}");
            return Ok(await _configService.GetConfigs(key));
        }

        [HttpPost]
        [Route("save")]
        public async Task<IActionResult> SaveConfigs([FromBody] List<SaveConfigRequest> request)
        {
            Log.Debug($"Config/SaveConfigs start: {JsonConvert.SerializeObject(request)}");
            return Ok(await _configService.Save(_mapper.Map<List<Config>>(request)));
        }
    }
}
