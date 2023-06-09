using Microsoft.AspNetCore.Http;

namespace BPHN.ModelLayer.ObjectQueues
{
    public class UploadFileParameter
    {
        public IFormFile File { get; set; }
        public string Id { get; set; }
    }
}
