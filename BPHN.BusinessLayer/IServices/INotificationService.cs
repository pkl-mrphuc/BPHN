using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface INotificationService
    {
        Task<ServiceResultModel> GetTopFiveNewNotifications();
        Task PushNotification(Account context, NotificationTypeEnum type);
    }
}
