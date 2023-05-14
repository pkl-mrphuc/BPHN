using BPHN.BusinessLayer.ImpServices.MailBuilders;
using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.ImpServices
{
    public class MailBuilderFactory : IMailBuilderFactory
    {
        private readonly IMailBuilder _resetPasswordMail;
        public MailBuilderFactory(IServiceProvider provider)
        {
            _resetPasswordMail = (IMailBuilder)provider.GetService(typeof(ResetPasswordMailBuilder));
        }
        public IMailBuilder GetInstance(MailTypeEnum mailType)
        {
            switch(mailType)
            {
                case MailTypeEnum.SET_PASSWORD:
                    return _resetPasswordMail;
            }

            throw new Exception("GetInstance Fail");
        }
    }
}
