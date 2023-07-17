using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface INotificationService
    {
        Task<ServiceResultModel> GetTopFiveNewNotifications();
        ServiceResultModel Insert<T>(Account context, NotificationTypeEnum type, T model);
    }
}
