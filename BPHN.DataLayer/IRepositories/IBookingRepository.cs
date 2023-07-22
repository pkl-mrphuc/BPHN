using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetById(string id);
        Task<bool> CheckFreeTimeFrame(Booking data);
        Task<bool> Insert(Booking data);
        Task<bool> Update(Booking data);
        Task<object> GetCountPaging(int pageIndex, int pageSize, Guid[] relationIds, string txtSearch);
        Task<List<Booking>> GetPaging(int pageIndex, int pageSize, Guid[] relationIds, string txtSearch, bool hasBookingDetail = false);
    }
}
