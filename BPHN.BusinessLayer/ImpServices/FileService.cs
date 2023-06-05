using BPHN.BusinessLayer.IServices;
using BPHN.IRabbitMQLayer;
using BPHN.ModelLayer;
using BPHN.ModelLayer.ObjectQueues;
using Google.Protobuf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.ImpServices
{
    public class FileService : IFileService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private AppSettings _appSettings;
        public FileService(IHostingEnvironment hostingEnvironment, IOptions<AppSettings> appSettings)
        {
            _hostingEnvironment = hostingEnvironment;
            _appSettings = appSettings.Value;
        }

        public ServiceResultModel GetLinkFile(string id)
        {

            if (string.IsNullOrEmpty(id))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không được để trống"
                };
            }

            string path = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder);
            DirectoryInfo dir = new DirectoryInfo(path);

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
                        Message = "File không tồn tại"
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
                        Message = "File không tồn tại"
                    };
                }

                fullName = files[0].Name;
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = string.Format("{0}{1}", _appSettings.FileUrl, fullName)
            };
        }

        public async Task<ServiceResultModel> UploadFile(IFormFile file, string id)
        {
            if (string.IsNullOrEmpty(id) || file == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không được để trống"
                };
            }

            string extension = Path.GetExtension(file.FileName);

            string[] validExtension = new string[3] { ".png", ".jpg", ".jpeg" };
            if (!validExtension.Contains(extension.ToLower()))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = "File định dạng không hợp lệ"
                };
            }

            string path = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.FileFolder, string.Format("{0}{1}", id, extension));
            if(File.Exists(path))
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EXISTED,
                    Message = "File đã tồn tại"
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
