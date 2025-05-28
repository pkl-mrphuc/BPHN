using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface INotificationService
    {
        Task Insert<T>(Account context, NotificationTypeEnum type, T model);
    }
}
