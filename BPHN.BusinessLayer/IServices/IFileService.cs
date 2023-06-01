using BPHN.ModelLayer;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.IServices
{
    public interface IFileService
    {
        ServiceResultModel UploadFile(IFormFile file, string id);
        ServiceResultModel GetLinkFile(string id);
    }
}
