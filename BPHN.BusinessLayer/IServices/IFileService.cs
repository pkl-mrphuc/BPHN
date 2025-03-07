using BPHN.ModelLayer;
using Microsoft.AspNetCore.Http;

namespace BPHN.BusinessLayer.IServices
{
    public interface IFileService
    {
        Task<ServiceResultModel> UploadFile(IFormFile file, string id);
        ServiceResultModel GetLinkFile(string id);
        string GetFileUrl(string id);
        ServiceResultModel DeleteFile(string id);
    }
}
