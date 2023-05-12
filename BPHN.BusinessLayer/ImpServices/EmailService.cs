using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.ImpServices
{
    public class EmailService : IEmailService
    {
        public bool SendMail(MailTypeEnum mailType)
        {
            return true;
        }
    }
}
