using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;

namespace BPHN.DataLayer.IRepositories
{
    public interface IBookingDetailRepository
    {
        Task<List<BookingDetail>> GetInRangeDate(Guid accountId, DateTime startDate, DateTime endDate);
        Task<bool> Cancel(string id);
        Task<IEnumerable<CalendarEvent>> GetByDate(string date, Guid accountId);
        Task<bool> UpdateMatch(CalendarEvent eventInfo);
        Task<IEnumerable<CalendarEvent>> GetEventsByRangeDate(DateTime startDate, DateTime endDate, Guid pitchId, string nameDetail);
        Task<List<CalendarEvent>> GetByRangeDate(string startDate, string endDate, string pitchId, string nameDetail);
        Task<List<BookingDetail>> GetByBookingId(Guid bookingId);
        Task<BookingDetail?> GetById(string id);
    }
}
