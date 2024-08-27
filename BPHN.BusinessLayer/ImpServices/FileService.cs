using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BPHN.BusinessLayer.ImpServices
{
    public class FileService : IFileService
    {
        [Obsolete]
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IResourceService _resourceService;
        private AppSettings _appSettings;

        [Obsolete]
        public FileService(IHostingEnvironment hostingEnvironment, IOptions<AppSettings> appSettings, IResourceService resourceService)
        {
            _hostingEnvironment = hostingEnvironment;
            _appSettings = appSettings.Value;
            _resourceService = resourceService;
        }

        [Obsolete]
        public ServiceResultModel DeleteFile(string id)
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

            var pathPng = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, $"{id}.png");
            var pathJpg = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, $"{id}.jpg");
            var pathJpeg = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, $"{id}.jpeg");

            if (File.Exists(pathPng))
            {
                File.Delete(pathPng);
            }
            else if (File.Exists(pathJpg))
            {
                File.Delete(pathJpg);
            }
            else if (File.Exists(pathJpeg))
            {
                File.Delete(pathJpeg);
            }

            return new ServiceResultModel
            {
                Success = true
            };
        }

        [Obsolete]
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

            var pathPng = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, $"{id}.png");
            var pathJpg = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, $"{id}.jpg");
            var pathJpeg = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, $"{id}.jpeg");

            var fullName = string.Empty;
            if (File.Exists(pathPng))
            {
                fullName = $"{id}.png";
            }
            else if (File.Exists(pathJpg))
            {
                fullName = $"{id}.jpg";
            }
            else if (File.Exists(pathJpeg))
            {
                fullName = $"{id}.jpeg";
            }
            else
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = _resourceService.Get(SharedResourceKey.NOTEXIST)
                };
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = $"{_appSettings.FileUrl}{fullName}"
            };
        }

        [Obsolete]
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

            DeleteFile(id);

            var path = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, $"{id}{extension}");
            using var fileStream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(fileStream);

            return new ServiceResultModel()
            {
                Success = true,
                Data = $"{_appSettings.FileUrl}{id}{extension}"
            };
        }
    }
}
