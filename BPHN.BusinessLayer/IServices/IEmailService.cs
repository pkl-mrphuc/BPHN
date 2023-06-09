using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IEmailService
    {
        bool SendMail(ObjectQueue objQueue);
    }
}
