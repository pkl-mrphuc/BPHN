using BPHN.BusinessLayer.IServices;
using Microsoft.AspNetCore.Mvc;

namespace BPHN.WebAPI.Controllers
{
    public class FilesController : BaseController
    {
        private readonly IFileService _fileService;
        public FilesController(IServiceProvider provider)
        {
            _fileService = provider.GetRequiredService<IFileService>();
        }

        [HttpPost]
        [Route("upload/{id}")]
        public IActionResult UploadFile([FromForm] IFormFile file, string id)
        {
            return Ok(_fileService.UploadFile(file, id));
        }

        [HttpGet]
        [Route("get/{id}")]
        public IActionResult GetLinkFile(string id)
        {
            return Ok(_fileService.GetLinkFile(id));
        }
    }
}
