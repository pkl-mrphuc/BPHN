using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    [ApiAuthorize]
    public class ConfigsController : BaseController
    {
        private readonly IConfigService _configService;
        public ConfigsController(IServiceProvider provider)
        {
            _configService = provider.GetRequiredService<IConfigService>();
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetConfigs(string key)
        {
            return Ok(_configService.GetConfigs(key));
        }

        [HttpPost]
        [Route("save")]
        public IActionResult SaveConfigs([FromBody] List<Config> configs)
        {
            return Ok(_configService.Save(configs));
        }
    }
}
