using BPHN.BusinessLayer.IServices;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    public class FilesController : BaseController
    {
        private readonly IFileService _fileService;
        public FilesController(IServiceProvider provider) : base(provider)
        {
            _fileService = provider.GetRequiredService<IFileService>();
        }

        [HttpPost]
        [Route("upload/{id}")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, string id)
        {
            return Ok(await _fileService.UploadFile(file, id));
        }

        [HttpGet]
        [Route("get/{id}")]
        public IActionResult GetLinkFile(string id)
        {
            return Ok(_fileService.GetLinkFile(id));
        }
    }
}
