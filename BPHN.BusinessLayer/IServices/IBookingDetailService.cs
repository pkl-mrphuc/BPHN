using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;

namespace BPHN.BusinessLayer.IServices
{
    public interface IBookingDetailService
    {
        ServiceResultModel GetMatchDatesByWeekendays(DateTime startDate, DateTime endDate, int weekendays);
        ServiceResultModel GetMatchDates(DateTime startDate, DateTime endDate);
        Task<ServiceResultModel> Cancel(string id);
        Task<ServiceResultModel> GetByDate(string date);
        Task<ServiceResultModel> UpdateMatch(CalendarEvent eventInfo);
        Task<ServiceResultModel> GetByRangeDate(string startDate, string endDate, string pitchId, string nameDetail);
    }
}
