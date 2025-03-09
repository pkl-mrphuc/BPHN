using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Responses;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BPHN.BusinessLayer.ImpServices
{
    public class BookingDetailService : BaseService, IBookingDetailService
    {
        private readonly IBookingDetailRepository _bookingDetailRepository;
        private readonly INotificationService _notificationService;
        private readonly IHistoryLogService _historyLogService;
        private readonly IPermissionService _permissionService;
        public BookingDetailService(
            IServiceProvider serviceProvider,
            IOptions<AppSettings> appSettings,
            INotificationService notificationService,
            IHistoryLogService historyLogService,
            IPermissionService permissionService,
            IBookingDetailRepository bookingDetailRepository) : base(serviceProvider, appSettings)
        {
            _bookingDetailRepository = bookingDetailRepository;
            _notificationService = notificationService;
            _historyLogService = historyLogService;
            _permissionService = permissionService;
        }

        public async Task<ServiceResultModel> Cancel(string id)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = _resourceService.Get(SharedResourceKey.OUTTIME)
                };
            }

            var hasPermission = await _permissionService.IsValidPermission(context.Id, FunctionTypeEnum.EDITBOOKING);
            if (!hasPermission)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            var oldData = await _bookingDetailRepository.GetById(id);
            if (oldData is null)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = _resourceService.Get(SharedResourceKey.NOTEXIST, context.LanguageConfig)
                };
            }

            var result = await _bookingDetailRepository.Cancel(id);
            if (result)
            {
                await _notificationService.Insert<BookingDetail>(context, NotificationTypeEnum.CANCELBOOKINGDETAIL, new BookingDetail
                {
                    MatchCode = oldData.MatchCode
                });

                var fakeNewData = oldData;
                fakeNewData.Status = BookingStatusEnum.CANCEL.ToString();
                _historyLogService.Write(Guid.NewGuid(),
                    new HistoryLog
                    {
                        ActionType = ActionEnum.CANCEL,
                        Entity = EntityEnum.BOOKINGDETAIL.ToString(),
                        Data = new HistoryLogDescription
                        {
                            ModelId = oldData.Id,
                            OldData = JsonConvert.SerializeObject(oldData),
                            NewData = JsonConvert.SerializeObject(fakeNewData)
                        }
                    }, context);
            }
            return new ServiceResultModel
            {
                Success = result
            };
        }

        public async Task<ServiceResultModel> UpdateMatch(CalendarEvent eventInfo)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = _resourceService.Get(SharedResourceKey.OUTTIME)
                };
            }

            var hasPermission = await _permissionService.IsValidPermission(context.Id, FunctionTypeEnum.EDITBOOKING);
            if (!hasPermission)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            var oldData = await _bookingDetailRepository.GetById(eventInfo.Id.ToString());
            if (oldData is null)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = _resourceService.Get(SharedResourceKey.NOTEXIST, context.LanguageConfig)
                };
            }

            var result = await _bookingDetailRepository.UpdateMatch(eventInfo);
            if (result)
            {
                await _notificationService.Insert<BookingDetail>(context, NotificationTypeEnum.UPDATEMATCH, new BookingDetail()
                {
                    MatchCode = oldData.MatchCode
                });

                var fakeNewData = oldData;
                fakeNewData.TeamA = eventInfo.TeamA;
                fakeNewData.TeamB = eventInfo.TeamB;
                fakeNewData.Note = eventInfo.Note;
                fakeNewData.Deposite = eventInfo.Deposite;
                _historyLogService.Write(Guid.NewGuid(),
                    new HistoryLog
                    {
                        ActionType = ActionEnum.UPDATE,
                        Entity = EntityEnum.BOOKINGDETAIL.ToString(),
                        Data = new HistoryLogDescription
                        {
                            ModelId = oldData.Id,
                            OldData = JsonConvert.SerializeObject(oldData),
                            NewData = JsonConvert.SerializeObject(fakeNewData)
                        }
                    }, context);
            }
            return new ServiceResultModel
            {
                Success = result
            };
        }

        public async Task<ServiceResultModel> GetByDate(string date)
        {
            var context = _contextService.GetContext();
            if (context is null)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.OUT_TIME,
                    Message = _resourceService.Get(SharedResourceKey.OUTTIME)
                };
            }

            var hasPermission = await _permissionService.IsValidPermission(context.Id, FunctionTypeEnum.VIEWLISTBOOKINGDETAIL);
            if (!hasPermission)
            {
                return new ServiceResultModel
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
            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<List<CalendarEventRespond>>(result)
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
            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<List<CalendarEventRespond>>(result)
            };
        }

        public async Task<List<BookingDetail>> GetByBookingId(Guid bookingId)
        {
            return await _bookingDetailRepository.GetByBookingId(bookingId);
        }

        public async Task<List<BookingDetail>> GetInRangeDate(Guid accountId, DateTime startDate, DateTime endDate)
        {
            return await _bookingDetailRepository.GetInRangeDate(accountId, startDate, endDate);
        }

        public List<BookingDetail> GetMatchDates(DateTime startDate, DateTime endDate)
        {
            var lstBookingDetail = new List<BookingDetail>();
            while (startDate <= endDate)
            {
                lstBookingDetail.Add(new BookingDetail
                {
                    MatchDate = startDate
                });
                startDate = startDate.AddDays(1);
            }

            return lstBookingDetail;
        }

        public List<BookingDetail> GetMatchDatesByWeekdays(DateTime startDate, DateTime endDate, int weekendays)
        {
            var lstBookingDetail = new List<BookingDetail>();
            while (startDate <= endDate)
            {
                if ((int)startDate.DayOfWeek == weekendays)
                {
                    lstBookingDetail.Add(new BookingDetail
                    {
                        MatchDate = startDate
                    });
                    startDate = startDate.AddDays(7);
                }
                else
                {
                    startDate = startDate.AddDays(1);
                }
            }

            return lstBookingDetail;
        }
    }
}
