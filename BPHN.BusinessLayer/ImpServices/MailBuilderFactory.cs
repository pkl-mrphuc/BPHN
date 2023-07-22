using BPHN.BusinessLayer.ImpServices.MailBuilders;
using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.ImpServices
{
    public class MailBuilderFactory : IMailBuilderFactory
    {
        private readonly IMailBuilder _forgotPasswordMail;
        private readonly IMailBuilder _setPasswordMail;
        private readonly IMailBuilder _approvalBookingMail;
        private readonly IMailBuilder _declineBookingMail;
        public MailBuilderFactory(IServiceProvider provider)
        {
            _forgotPasswordMail = (IMailBuilder)provider.GetService(typeof(ForgotPasswordMailBuilder));
            _setPasswordMail = (IMailBuilder)provider.GetService(typeof(SetPasswordMailBuilder));
            _approvalBookingMail = (IMailBuilder)provider.GetService(typeof(ApprovalBookingMailBuilder));
            _declineBookingMail = (IMailBuilder)provider.GetService(typeof(DeclineBookingMailBuilder));
        }
        public IMailBuilder GetInstance(MailTypeEnum mailType)
        {
            switch(mailType)
            {
                case MailTypeEnum.FORTGOTPASSWORD:
                    return _forgotPasswordMail;
                case MailTypeEnum.SETPASSWORD:
                    return _setPasswordMail;
                case MailTypeEnum.APPROVALBOOKING:
                    return _approvalBookingMail;
                case MailTypeEnum.DECLINEBOOKING:
                    return _declineBookingMail;
            }

            throw new Exception("GetInstance Fail");
        }
    }
}
