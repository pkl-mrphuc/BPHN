using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;

namespace BPHN.BusinessLayer.ImpServices
{
    public class BookingDetailService : BaseService, IBookingDetailService
    {
        private readonly IContextService _contextService;
        private readonly IBookingDetailRepository _bookingDetailRepository;
        public BookingDetailService(IContextService contextService, 
            IBookingDetailRepository bookingDetailRepository)
        {
            _contextService = contextService;
            _bookingDetailRepository = bookingDetailRepository;
        }

        public async Task<ServiceResultModel> Cancel(string id)
        {
            var context = _contextService.GetContext();
            if (context == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = "Token đã hết hạn"
                };
            }

            var result = await _bookingDetailRepository.Cancel(id);
            return new ServiceResultModel()
            {
                Success = result
            };
        }

        public async Task<ServiceResultModel> GetByDate(string date)
        {
            var context = _contextService.GetContext();
            if (context == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = "Token đã hết hạn"
                };
            }

            var result = await _bookingDetailRepository.GetByDate(date, context.Id);
            result = result.Select(item =>
            {
                item.Start = new DateTime(item.MatchDate.Year, item.MatchDate.Month, item.MatchDate.Day, item.Start.Hour, item.Start.Minute, 0);
                item.End = new DateTime(item.MatchDate.Year, item.MatchDate.Month, item.MatchDate.Day, item.End.Hour, item.End.Minute, 0);
                return item;
            }).ToList();
            return new ServiceResultModel()
            {
                Success = true,
                Data = result
            };
        }

        public async Task<ServiceResultModel> GetByRangeDate(string startDate, string endDate, string pitchId, string nameDetail)
        {
            var result = await _bookingDetailRepository.GetByRangeDate(startDate, endDate, pitchId, nameDetail);
            result = result.Select(item =>
            {
                item.Start = new DateTime(item.MatchDate.Year, item.MatchDate.Month, item.MatchDate.Day, item.Start.Hour, item.Start.Minute, 0);
                item.End = new DateTime(item.MatchDate.Year, item.MatchDate.Month, item.MatchDate.Day, item.End.Hour, item.End.Minute, 0);
                return item;
            }).ToList();
            return new ServiceResultModel()
            {
                Success = true,
                Data = result
            };
        }

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

        public async Task<ServiceResultModel> UpdateMatch(CalendarEvent eventInfo)
        {
            var context = _contextService.GetContext();
            if (context == null)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = "Token đã hết hạn"
                };
            }

            var result = await _bookingDetailRepository.UpdateMatch(eventInfo);
            return new ServiceResultModel()
            {
                Success = result
            };
        }
    }
}
