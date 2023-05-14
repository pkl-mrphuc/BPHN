using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.ImpServices.MailBuilders
{
    public class ResetPasswordMailBuilder : IMailBuilder
    {
        public List<Attachment> BuildAttachments(object? data)
        {
            return new List<Attachment>();
        }

        public string BuildBody(object? data)
        {
            return "";
        }

        public string BuildSubject(object? data)
        {
            return "[BPHN] Reset Your Password";
        }
    }
}
