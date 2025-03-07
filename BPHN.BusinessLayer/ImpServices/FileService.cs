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

        public ServiceResultModel DeleteFile(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
                };
            }

            var result = true;

            var pathPng = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, $"{id}.png");
            var pathJpg = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, $"{id}.jpg");
            var pathJpeg = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, $"{id}.jpeg");

            try
            {
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
            }
            catch (Exception)
            {
                result = false;
            }

            return new ServiceResultModel
            {
                Success = result
            };
        }

        public ServiceResultModel GetLinkFile(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
                };
            }

            var fileUrl = GetFileUrl(id);
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = _resourceService.Get(SharedResourceKey.NOTEXIST)
                };
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = fileUrl
            };
        }

        public async Task<ServiceResultModel> UploadFile(IFormFile file, string id)
        {
            if (string.IsNullOrWhiteSpace(id) || file is null)
            {
                return new ServiceResultModel
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
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDDATA)
                };
            }

            if (DeleteFile(id).Success)
            {
                var path = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, $"{id}{extension}");
                using var fileStream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(fileStream);

                return new ServiceResultModel
                {
                    Success = true,
                    Data = $"{_appSettings.FileUrl}{id}{extension}"
                };
            }
            else
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE
                };
            }
        }

        public string GetFileUrl(string id)
        {
            var pathPng = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, $"{id}.png");
            var pathJpg = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, $"{id}.jpg");
            var pathJpeg = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, $"{id}.jpeg");

            string? fullName = null;
            if (File.Exists(pathPng))
            {
                fullName = $"{_appSettings.FileUrl}{id}.png";
            }
            else if (File.Exists(pathJpg))
            {
                fullName = $"{_appSettings.FileUrl}{id}.jpg";
            }
            else if (File.Exists(pathJpeg))
            {
                fullName = $"{_appSettings.FileUrl}{id}.jpeg";
            }

            return fullName ?? string.Empty;
        }
    }
}
