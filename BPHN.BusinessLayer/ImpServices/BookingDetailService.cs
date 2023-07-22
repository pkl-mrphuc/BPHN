using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Microsoft.Extensions.Options;

namespace BPHN.BusinessLayer.ImpServices
{
    public class BookingDetailService : BaseService, IBookingDetailService
    {
        private readonly IBookingDetailRepository _bookingDetailRepository;
        private readonly INotificationService _notificationService;
        public BookingDetailService(
            IServiceProvider serviceProvider,
            IOptions<AppSettings> appSettings,
            INotificationService notificationService,
            IBookingDetailRepository bookingDetailRepository) : base(serviceProvider, appSettings)
        {
            _bookingDetailRepository = bookingDetailRepository;
            _notificationService = notificationService;
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
                    Message = _resourceService.Get(SharedResourceKey.OUTTIME)
                };
            }

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.EDITBOOKING);
            if (!hasPermission)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            var result = await _bookingDetailRepository.Cancel(id);
            if(result)
            {
                _notificationService.Insert<string>(context, NotificationTypeEnum.CANCELBOOKINGDETAIL, id);
            }
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
                    Message = _resourceService.Get(SharedResourceKey.OUTTIME)
                };
            }

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.VIEWLISTBOOKINGDETAIL);
            if (!hasPermission)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            var result = await _bookingDetailRepository.GetByDate(date, context.RelationIds.ToArray());
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

        public List<BookingDetail> GetMatchDates(DateTime startDate, DateTime endDate)
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

            return lstBookingDetail;
        }

        public List<BookingDetail> GetMatchDatesByWeekendays(DateTime startDate, DateTime endDate, int weekendays)
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

            return lstBookingDetail;
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
                    Message = _resourceService.Get(SharedResourceKey.OUTTIME)
                };
            }

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.EDITBOOKING);
            if(!hasPermission)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            var result = await _bookingDetailRepository.UpdateMatch(eventInfo);
            if(result)
            {
                _notificationService.Insert<CalendarEvent>(context, NotificationTypeEnum.UPDATEMATCH, eventInfo);
            }
            return new ServiceResultModel()
            {
                Success = result
            };
        }
    }
}
