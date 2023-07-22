using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BPHN.BusinessLayer.ImpServices
{
    public class FileService : IFileService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IResourceService _resourceService;
        private AppSettings _appSettings;
        public FileService(IHostingEnvironment hostingEnvironment, IOptions<AppSettings> appSettings, IResourceService resourceService)
        {
            _hostingEnvironment = hostingEnvironment;
            _appSettings = appSettings.Value;
            _resourceService = resourceService;
        }

        public ServiceResultModel GetLinkFile(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
                };
            }

            var path = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder);
            var dir = new DirectoryInfo(path);

            var fullName = string.Empty;

            if (!id.Contains(".png") &&
                !id.Contains(".jpg") &&
                !id.Contains(".jpeg"))
            {

                var filesPng = dir.GetFiles(id + ".png");
                var filesJpg = dir.GetFiles(id + ".jpg");
                var filesJpeg = dir.GetFiles(id + ".jpeg");

                if(filesJpeg.Length == 0 && 
                    filesPng.Length == 0 &&
                    filesJpg.Length == 0)
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NOT_EXISTS,
                        Message = _resourceService.Get(SharedResourceKey.NOTEXIST)
                    };
                }

                if(filesPng.Length > 0)
                {
                    fullName = filesPng[0].Name;
                }
                if(filesJpeg.Length > 0)
                {
                    fullName = filesJpeg[0].Name;
                }
                if(filesJpg.Length > 0)
                {
                    fullName = filesJpg[0].Name;
                }
            }
            else
            {
                var files = dir.GetFiles(id);
                if(files.Length == 0)
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NOT_EXISTS,
                        Message = _resourceService.Get(SharedResourceKey.NOTEXIST)
                    };
                }

                fullName = files[0].Name;
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = $"{_appSettings.FileUrl}{fullName}"
            };
        }

        public async Task<ServiceResultModel> UploadFile(IFormFile file, string id)
        {
            if (string.IsNullOrWhiteSpace(id) || file == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
                };
            }

            var extension = Path.GetExtension(file.FileName);

            var validExtension = new string[3] { ".png", ".jpg", ".jpeg" };
            if (!validExtension.Contains(extension.ToLower()))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDDATA)
                };
            }

            var path = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, string.Format("{0}{1}", id, extension));
            if(File.Exists(path))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EXISTED,
                    Message = _resourceService.Get(SharedResourceKey.EXISTED)
                };
            }

            var fileStream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return new ServiceResultModel()
            {
                Success = true
            };
        }
    }
}
