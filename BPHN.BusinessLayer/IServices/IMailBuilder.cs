using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.IServices
{
    public interface IMailBuilder
    {
        string BuildSubject(object? data);
        Task<string> BuildBody(object? data);
        List<Attachment> BuildAttachments(object? data);
    }
}
