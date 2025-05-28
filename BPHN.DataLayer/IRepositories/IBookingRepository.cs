using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;

namespace BPHN.DataLayer.IRepositories
{
    public interface IBookingRepository
    {
        Task<List<Booking>> GetByIds(string ids);
        Task<Booking> GetById(Guid id);
        Task<bool> CheckFreeTimeFrame(Booking data);
        Task<bool> Insert(Booking data);
        Task<bool> Update(Booking data);
        Task<object> GetCountPaging(GetBookingPagingModel model);
        Task<IEnumerable<BookingManager>> GetPaging(GetBookingPagingModel model);
    }
}
