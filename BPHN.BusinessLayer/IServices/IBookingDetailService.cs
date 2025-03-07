using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;

namespace BPHN.BusinessLayer.IServices
{
    public interface IBookingDetailService
    {
        Task<ServiceResultModel> Cancel(string id);
        Task<ServiceResultModel> GetByDate(string date);
        Task<ServiceResultModel> UpdateMatch(CalendarEvent eventInfo);
        Task<ServiceResultModel> GetByRangeDate(string startDate, string endDate, string pitchId, string nameDetail);
        List<BookingDetail> GetMatchDatesByWeekdays(DateTime startDate, DateTime endDate, int weekendays);
        List<BookingDetail> GetMatchDates(DateTime startDate, DateTime endDate);
        Task<List<BookingDetail>> GetByBookingId(Guid bookingId);
        Task<List<BookingDetail>> GetInRangeDate(Guid accountId, DateTime startDate, DateTime endDate);
    }
}
