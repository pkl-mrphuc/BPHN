using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.ModelLayer.ObjectQueues
{
    public class UploadFileParameter
    {
        public IFormFile File { get; set; }
        public string Id { get; set; }
    }
}
