using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IEmailService
    {
        bool SendMail<T>(string type, T data);
    }
}
