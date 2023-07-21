using BPHN.BusinessLayer.ImpServices.MailBuilders;
using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.ImpServices
{
    public class MailBuilderFactory : IMailBuilderFactory
    {
        private readonly IMailBuilder _forgotPasswordMail;
        private readonly IMailBuilder _setPasswordMail;
        public MailBuilderFactory(IServiceProvider provider)
        {
            _forgotPasswordMail = (IMailBuilder)provider.GetService(typeof(ForgotPasswordMailBuilder));
            _setPasswordMail = (IMailBuilder)provider.GetService(typeof(SetPasswordMailBuilder));
        }
        public IMailBuilder GetInstance(MailTypeEnum mailType)
        {
            switch(mailType)
            {
                case MailTypeEnum.FORTGOT_PASSWORD:
                    return _forgotPasswordMail;
                case MailTypeEnum.SET_PASSWORD:
                    return _setPasswordMail;
            }

            throw new Exception("GetInstance Fail");
        }
    }
}
