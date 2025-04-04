using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface INotificationService
    {
        Task<ServiceResultModel> GetTopFiveNewNotifications();
        Task Insert<T>(Account context, NotificationTypeEnum type, T model);
    }
}
