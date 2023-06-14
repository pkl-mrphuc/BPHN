using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetById(string id);
        Task<bool> CheckFreeTimeFrame(Booking data);
        Task<bool> Insert(Booking data);
        Task<object> GetCountPaging(int pageIndex, int pageSize, Guid accountId, string txtSearch);
        Task<List<Booking>> GetPaging(int pageIndex, int pageSize, Guid accountId, string txtSearch, bool hasBookingDetail = false);
    }
}
