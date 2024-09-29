using BPHN.BusinessLayer.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    [AllowAnonymous]
    public class ResourcesController : BaseController
    {
        private readonly IResourceService _resourceService;
        public ResourcesController(IServiceProvider provider) : base(provider)
        {
            _resourceService = provider.GetRequiredService<IResourceService>();
        }

        [HttpGet]
        public IActionResult Get(string key, string lang)
        {
            return Ok(_resourceService.Get(key, lang));
        }
    }
}
