using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IMailBuilderFactory
    {
        IMailBuilder GetInstance(MailTypeEnum mailType);
    }
}
