using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IWsSendService
    {
        Task PushNotification(int type, string model);
    }
}
