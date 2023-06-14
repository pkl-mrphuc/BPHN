using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.ImpServices
{
    public class BookingDetailService : BaseService, IBookingDetailService
    {
        public ServiceResultModel GetMatchDates(DateTime startDate, DateTime endDate)
        {
            var lstBookingDetail = new List<BookingDetail>();
            while (startDate <= endDate)
            {
                lstBookingDetail.Add(new BookingDetail()
                {
                    MatchDate = startDate
                });
                startDate = startDate.AddDays(1);
            }
            return new ServiceResultModel()
            {
                Success = true,
                Data = lstBookingDetail
            };
        }

        public ServiceResultModel GetMatchDatesByWeekendays(DateTime startDate, DateTime endDate, int weekendays)
        {
            var lstBookingDetail = new List<BookingDetail>();
            while (startDate <= endDate)
            {
                if((int)startDate.DayOfWeek == weekendays)
                {
                    lstBookingDetail.Add(new BookingDetail()
                    {
                        MatchDate = startDate
                    });
                    startDate= startDate.AddDays(7);
                }
                else
                {
                    startDate = startDate.AddDays(1);
                }
            }
            return new ServiceResultModel()
            {
                Success = true,
                Data = lstBookingDetail
            };
        }
    }
}
