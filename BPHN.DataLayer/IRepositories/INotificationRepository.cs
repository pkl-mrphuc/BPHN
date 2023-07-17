using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;

namespace BPHN.DataLayer.IRepositories
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetTopFiveNewNotifications(List<WhereCondition> where);
        Task<bool> Insert(Notification notification);
    }
}
