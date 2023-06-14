using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IBookingDetailService
    {
        ServiceResultModel GetMatchDatesByWeekendays(DateTime startDate, DateTime endDate, int weekendays);
        ServiceResultModel GetMatchDates(DateTime startDate, DateTime endDate);
    }
}
