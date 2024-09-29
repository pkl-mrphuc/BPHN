using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.ObjectQueues;
using BPHN.ModelLayer.Others;
using BPHN.ModelLayer.Responses;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security;

namespace BPHN.BusinessLayer.ImpServices
{
    public class BookingService : BaseService, IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IHistoryLogService _historyLogService;
        private readonly IPitchRepository _pitchRepository;
        private readonly IBookingDetailRepository _bookingDetailRepository;
        private readonly IBookingDetailService _bookingDetailService;
        private readonly ITimeFrameInfoRepository _timeFrameInfoRepository;
        private readonly INotificationService _notificationService;
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailService _mailService;
        public BookingService(
            IServiceProvider serviceProvider,
            IOptions<AppSettings> appSettings,
            IBookingRepository bookingRepository,
            IHistoryLogService historyLogService,
            IPitchRepository pitchRepository,
            IBookingDetailRepository bookingDetailRepository,
            IBookingDetailService bookingDetailService,
            INotificationService notificationService,
            ITimeFrameInfoRepository timeFrameInfoRepository,
            IAccountRepository accountRepository,
            IEmailService mailService) : base(serviceProvider, appSettings)
        {
            _bookingRepository = bookingRepository;
            _historyLogService = historyLogService;
            _bookingDetailRepository = bookingDetailRepository;
            _pitchRepository = pitchRepository;
            _bookingDetailService = bookingDetailService;
            _timeFrameInfoRepository = timeFrameInfoRepository;
            _notificationService = notificationService;
            _accountRepository = accountRepository;
            _mailService = mailService;
        }

        public async Task<ServiceResultModel> CheckFreeTimeFrame(Booking data)
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

            var hasPermissionAdd = await IsValidPermission(context.Id, FunctionTypeEnum.ADDBOOKING);
            var hasPermissionEdit = await IsValidPermission(context.Id, FunctionTypeEnum.EDITBOOKING);
            if (!hasPermissionAdd && !hasPermissionEdit)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            if (data.IsRecurring)
            {
                data.BookingDetails = _bookingDetailService.GetMatchDatesByWeekdays(data.StartDate, data.EndDate, data.Weekendays ?? (int)DayOfWeek.Monday);
            }
            else
            {
                data.BookingDetails = new List<BookingDetail>
                {
                    new BookingDetail
                    {
                        MatchDate = data.StartDate
                    }
                };
            }

            var result = await _bookingRepository.CheckFreeTimeFrame(data);
            return new ServiceResultModel
            {
                Success = result,
                Message = !result ? _resourceService.Get(SharedResourceKey.EXISTED, context.LanguageConfig) : string.Empty
            };

        }

        public async Task<ServiceResultModel> FindBlank(Booking data)
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

            var hasPermissionAdd = await IsValidPermission(context.Id, FunctionTypeEnum.ADDBOOKING);
            var hasPermissionEdit = await IsValidPermission(context.Id, FunctionTypeEnum.EDITBOOKING);
            if (!hasPermissionAdd && !hasPermissionEdit)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            var lstResult = new List<Booking>();
            var lstTimeFramesInADay = await GetAllTimeFrameInfoCanBookInADay(context.Id.ToString());

            if (lstTimeFramesInADay.Count > 0)
            {
                var now = DateTime.Now;
                if (data.StartDate < now)
                {
                    data.StartDate = now;
                }
                data.EndDate = new DateTime(now.Year, 12, 31, 23, 59, 59);
                var lstBooked = await GetDictionaryBookedByDate(context.Id, data.StartDate, data.EndDate);

                var lstBooking = new List<Booking>();
                // lay lich recurring tu ngay => den ngay
                if (data.IsRecurring)
                {
                    for (int i = 0; i <= 6; i++)
                    {
                        var lstDateByWeekendays = _bookingDetailService.GetMatchDatesByWeekdays(data.StartDate, data.EndDate, i);
                        for (int j = 0; j < lstTimeFramesInADay.Count; j++)
                        {
                            var clone = (Booking)lstTimeFramesInADay[j].Clone();
                            clone.Id = Guid.NewGuid();
                            clone.IsRecurring = true;
                            clone.BookingDate = DateTime.Now;
                            clone.StartDate = data.StartDate;
                            clone.EndDate = data.EndDate;
                            clone.PitchName = clone.Pitch is null ? string.Empty : clone.Pitch.Name;
                            clone.TimeFrameInfoName = clone.TimeFrameInfo is null ? string.Empty : clone.TimeFrameInfo.Name;
                            clone.BookingDetails = lstDateByWeekendays;
                            clone.Weekendays = i;
                            lstBooking.Add(clone);
                        }
                    }
                }
                // lay lich theo ngay tu ngay => den ngay
                else
                {
                    var lstDate = _bookingDetailService.GetMatchDates(data.StartDate, data.EndDate);
                    for (int i = 0; i < lstDate.Count; i++)
                    {
                        for (int j = 0; j < lstTimeFramesInADay.Count; j++)
                        {
                            var clone = (Booking)lstTimeFramesInADay[j].Clone();
                            clone.Id = Guid.NewGuid();
                            clone.IsRecurring = false;
                            clone.BookingDate = DateTime.Now;
                            clone.StartDate = lstDate[i].MatchDate;
                            clone.EndDate = lstDate[i].MatchDate;
                            clone.PitchName = clone.Pitch is null ? string.Empty : clone.Pitch.Name;
                            clone.TimeFrameInfoName = clone.TimeFrameInfo is null ? string.Empty : clone.TimeFrameInfo.Name;
                            clone.BookingDetails = new List<BookingDetail>() { lstDate[i] };
                            clone.Weekendays = (int)lstDate[i].MatchDate.DayOfWeek;
                            lstBooking.Add(clone);
                        }
                    }
                }

                var setMatchDate = new HashSet<string>(lstBooked.Select(item => item.Item4.ToString("dd/MM/yyyy")));
                for (int i = 0; i < lstBooking.Count; i++)
                {
                    var a = lstBooking[i];
                    var isConflict = lstBooked.Where(b =>
                                                        b.Item1 == a.PitchId &&
                                                        b.Item2 == a.TimeFrameInfoId &&
                                                        b.Item3 == a.NameDetail &&
                                                        a.BookingDetails.Where(c => setMatchDate.Contains(c.MatchDate.ToString("dd/MM/yyyy"))).FirstOrDefault() is not null
                                                    ).FirstOrDefault() is not null ? true : false;

                    if (!isConflict)
                    {
                        lstResult.Add(a);
                    }
                }
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = lstResult
            };
        }

        public async Task<ServiceResultModel> GetCountPaging(int pageIndex, int pageSize, string txtSearch)
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

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.VIEWLISTBOOKING);
            if (!hasPermission)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            if (pageIndex < 0) pageIndex = 1;
            if (pageSize > 100 || pageSize <= 0) pageSize = 50;

            var result = await _bookingRepository.GetCountPaging(pageIndex, pageSize, context.RelationIds.ToArray(), txtSearch);
            return new ServiceResultModel
            {
                Success = true,
                Data = result
            };
        }

        public async Task<ServiceResultModel> GetCountPagingV1(int pageIndex, int pageSize, string txtSearch)
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

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.VIEWLISTBOOKING);
            if (!hasPermission)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            if (pageIndex < 0) pageIndex = 1;
            if (pageSize > 100 || pageSize <= 0) pageSize = 50;

            var result = await _bookingRepository.GetCountPagingV1(pageIndex, pageSize, context.RelationIds.ToArray(), txtSearch);
            return new ServiceResultModel
            {
                Success = true,
                Data = result
            };
        }

        public async Task<ServiceResultModel> GetInstance(string id)
        {
            Booking? data = null;

            if (string.IsNullOrWhiteSpace(id))
            {
                data = new Booking();
                data.Id = Guid.NewGuid();
                data.BookingDate = DateTime.Now;
                data.StartDate = DateTime.Now;
                data.EndDate = DateTime.Now;
                data.IsRecurring = false;
                data.Weekendays = (int)DateTime.Now.DayOfWeek;
            }
            else
            {
                var lstBook = await _bookingRepository.GetById(id);
                data = lstBook.FirstOrDefault();
                if (data is null)
                {
                    return new ServiceResultModel
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NOT_EXISTS,
                        Message = _resourceService.Get(SharedResourceKey.NOTEXIST)
                    };
                }


            }
            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<BookingRespond>(data)
            };
        }

        public async Task<ServiceResultModel> GetPaging(int pageIndex, int pageSize, string txtSearch, bool hasBookingDetail = false)
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

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.VIEWLISTBOOKING);
            if (!hasPermission)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            if (pageIndex < 0) pageIndex = 1;
            if (pageSize > 100 || pageSize <= 0) pageSize = 50;

            var lstBooking = await _bookingRepository.GetPaging(pageIndex, pageSize, context.RelationIds.ToArray(), txtSearch, hasBookingDetail);
            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<List<BookingRespond>>(lstBooking)
            };
        }

        public async Task<ServiceResultModel> GetPagingV1(int pageIndex, int pageSize, string txtSearch, bool hasBookingDetail = false)
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

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.VIEWLISTBOOKING);
            if (!hasPermission)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            if (pageIndex < 0) pageIndex = 1;
            if (pageSize > 100 || pageSize <= 0) pageSize = 50;

            var lstBooking = await _bookingRepository.GetPagingV1(pageIndex, pageSize, context.RelationIds.ToArray(), txtSearch, hasBookingDetail);
            return new ServiceResultModel
            {
                Success = true,
                Data = _mapper.Map<List<BookingRespond>>(lstBooking)
            };
        }

        public async Task<ServiceResultModel> Insert(Booking data)
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

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.ADDBOOKING);
            if (!hasPermission)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            var isValid = ValidateModelByAttribute(data);
            if (!isValid)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig)
                };
            }

            if ((data.IsRecurring && data.StartDate.Date.Equals(data.EndDate.Date)) ||
                (data.IsRecurring && (!data.Weekendays.HasValue || (data.Weekendays.HasValue && data.Weekendays.Value < 0) || (data.Weekendays.HasValue && data.Weekendays.Value > 6))) ||
                data.EndDate.Date < DateTime.Now.Date)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NO_INTEGRITY,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT, context.LanguageConfig)
                };
            }

            var checkFreeServiceResult = await CheckFreeTimeFrame(data);
            if (!checkFreeServiceResult.Success)
            {
                return checkFreeServiceResult;
            }

            data.Id = data.Id.Equals(Guid.Empty) ? Guid.NewGuid() : data.Id;
            data.BookingDate = DateTime.Now;
            data.Status = BookingStatusEnum.SUCCESS.ToString();
            data.AccountId = context.Id;
            data.CreatedDate = DateTime.Now;
            data.CreatedBy = context.FullName;
            data.ModifiedBy = context.FullName;
            data.ModifiedDate = DateTime.Now;

            data.BookingDetails = data.BookingDetails.Select(item =>
            {
                item.Id = item.Id.Equals(Guid.Empty) ? Guid.NewGuid() : item.Id;
                item.CreatedDate = DateTime.Now;
                item.CreatedBy = context.FullName;
                item.ModifiedBy = context.FullName;
                item.ModifiedDate = DateTime.Now;
                item.BookingId = data.Id;
                item.Status = BookingStatusEnum.SUCCESS.ToString();
                return item;
            }).ToList();

            var insertResult = await _bookingRepository.Insert(data);
            if (insertResult)
            {
                var pitch = await _pitchRepository.GetById(data.PitchId.HasValue ? data.PitchId.Value : Guid.Empty);
                var frame = await _timeFrameInfoRepository.GetById(data.TimeFrameInfoId.HasValue ? data.TimeFrameInfoId.Value : Guid.Empty);
                await _notificationService.Insert<Booking>(context, NotificationTypeEnum.INSERTBOOKING, new Booking()
                {
                    PhoneNumber = data.PhoneNumber,
                    NameDetail = data.NameDetail,
                    PitchName = pitch?.Name ?? string.Empty,
                    TimeFrameInfoName = $"{frame?.TimeBegin.ToString("hh:mm:ss") ?? string.Empty} - {frame?.TimeEnd.ToString("hh:mm:ss") ?? string.Empty}"
                });

                _historyLogService.Write(Guid.NewGuid(), new HistoryLog
                {
                    ActionType = ActionEnum.INSERT,
                    Entity = EntityEnum.BOOKING.ToString(),
                    Data = new HistoryLogDescription
                    {
                        ModelId = data.Id,
                        NewData = JsonConvert.SerializeObject(data)
                    }
                }, context);
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = insertResult
            };
        }

        public async Task<ServiceResultModel> InsertBookingRequest(BookingRequest data)
        {
            var fakeContext = new Account()
            {
                Id = data.AccountId,
                FullName = data.PhoneNumber,
                UserName = data.PhoneNumber,
                IPAddress = _contextService.GetIPAddress()
            };

            var isValid = ValidateModelByAttribute(data);
            if (!isValid)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = _resourceService.Get(SharedResourceKey.EMPTYINPUT)
                };
            }

            var checkFreeServiceResult = await _bookingRepository.CheckFreeTimeFrame(new Booking()
            {
                PitchId = data.PitchId,
                NameDetail = data.NameDetail,
                TimeFrameInfoId = data.TimeFrameInfoId,
                BookingDetails = new List<BookingDetail>() {
                    new BookingDetail()
                    {
                        MatchDate = data.StartDate
                    }
                }
            });
            if (!checkFreeServiceResult)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EXISTED,
                    Message = _resourceService.Get(SharedResourceKey.EXISTED)
                };
            }

            var booking = new Booking();
            booking.Id = data.Id.Equals(Guid.Empty) ? Guid.NewGuid() : data.Id;
            booking.PhoneNumber = data.PhoneNumber;
            booking.Email = data.Email;
            booking.BookingDate = data.BookingDate;
            booking.IsRecurring = false;
            booking.Status = BookingStatusEnum.PENDING.ToString();
            booking.AccountId = data.AccountId;
            booking.TimeFrameInfoId = data.TimeFrameInfoId;
            booking.PitchId = data.PitchId;
            booking.Weekendays = (int)data.BookingDate.DayOfWeek;
            booking.NameDetail = data.NameDetail;
            booking.StartDate = data.StartDate;
            booking.EndDate = data.EndDate;
            booking.CreatedDate = DateTime.Now;
            booking.CreatedBy = fakeContext.UserName;
            booking.ModifiedBy = fakeContext.UserName;
            booking.ModifiedDate = DateTime.Now;

            booking.BookingDetails = new List<BookingDetail>
            {
                new BookingDetail
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    CreatedBy = fakeContext.UserName,
                    ModifiedBy = fakeContext.UserName,
                    ModifiedDate = DateTime.Now,
                    BookingId = data.Id,
                    Status = BookingStatusEnum.PENDING.ToString(),
                    MatchDate = data.StartDate,
                    Weekendays = (int)data.StartDate.DayOfWeek,
                    Note = data.Note,
                    TeamA = data.TeamA
                }
            };

            var insertResult = await _bookingRepository.Insert(booking);
            if (insertResult)
            {
                var pitch = await _pitchRepository.GetById(booking.PitchId.HasValue ? booking.PitchId.Value : Guid.Empty);
                var frame = await _timeFrameInfoRepository.GetById(booking.TimeFrameInfoId.HasValue ? booking.TimeFrameInfoId.Value : Guid.Empty);
                var matchDate = booking.BookingDetails.Select(item => item.MatchDate.ToString("dd/MM/yyyy")).FirstOrDefault();
                var context = new Account()
                {
                    RelationIds = await _accountRepository.GetRelationIds(data.AccountId) ?? new List<Guid>() { data.AccountId },
                    Id = data.AccountId,
                    FullName = data.PhoneNumber
                };
                await _notificationService.Insert<Booking>(context, NotificationTypeEnum.INSERTBOOKING, new Booking()
                {
                    PhoneNumber = data.PhoneNumber,
                    NameDetail = data.NameDetail,
                    PitchName = pitch?.Name ?? string.Empty,
                    TimeFrameInfoName = frame is not null ? $"{frame.TimeBegin.ToString("hh:mm:ss")} - {frame.TimeEnd.ToString("hh:mm:ss")}" : string.Empty
                });

                _historyLogService.Write(Guid.NewGuid(), new HistoryLog
                {
                    ActionType = ActionEnum.INSERT,
                    Entity = EntityEnum.BOOKING.ToString(),
                    Data = new HistoryLogDescription
                    {
                        ModelId = data.Id,
                        NewData = JsonConvert.SerializeObject(data)
                    }
                }, fakeContext);
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = insertResult
            };
        }

        public async Task<ServiceResultModel> Update(string id, BookingStatusEnum status)
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

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.EDITBOOKING);
            if (!hasPermission)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = _resourceService.Get(SharedResourceKey.INVALIDROLE, context.LanguageConfig)
                };
            }

            var lstBook = await _bookingRepository.GetById(id);
            var oldData = lstBook.FirstOrDefault();
            if (oldData is null)
            {
                return new ServiceResultModel
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NOT_EXISTS,
                    Message = _resourceService.Get(SharedResourceKey.NOTEXIST, context.LanguageConfig)
                };
            }
            var lstBookingDetail = await _bookingDetailRepository.GetByBookingId(oldData.Id);

            if (status != BookingStatusEnum.CANCEL)
            {
                var checkFreeServiceResult = await CheckFreeTimeFrame(oldData);
                if (!checkFreeServiceResult.Success)
                {
                    return checkFreeServiceResult;
                }
            }

            var data = oldData;
            data.Status = status.ToString();
            data.ModifiedBy = context.FullName;
            data.ModifiedDate = DateTime.Now;
            data.BookingDetails = lstBookingDetail.Select(item =>
            {
                item.Status = status.ToString();
                item.ModifiedBy = context.FullName;
                item.ModifiedDate = DateTime.Now;
                return item;
            }).ToList();

            var updateResult = await _bookingRepository.Update(data);
            if (updateResult)
            {
                var pitch = await _pitchRepository.GetById(oldData.PitchId.HasValue ? oldData.PitchId.Value : Guid.Empty);
                var frame = await _timeFrameInfoRepository.GetById(oldData.TimeFrameInfoId.HasValue ? oldData.TimeFrameInfoId.Value : Guid.Empty);
                var matchDate = data.BookingDetails.Select(item => item.MatchDate.ToString("dd/MM/yyyy")).FirstOrDefault();

                if (status == BookingStatusEnum.CANCEL)
                {
                    _mailService.SendMail("bphn.email.decline-booking",
                        new DeclineBookingParameter
                        {
                            ReceiverAddress = data.Email,
                            MailType = MailTypeEnum.DECLINEBOOKING,
                            ParameterType = typeof(DeclineBookingParameter),
                            PhoneNumber = data.PhoneNumber,
                            Reason = ""
                        });

                    await _notificationService.Insert<Booking>(context, NotificationTypeEnum.DECLINEBOOKING, new Booking()
                    {
                        PhoneNumber = data.PhoneNumber,
                        NameDetail = data.NameDetail,
                        PitchName = pitch?.Name ?? string.Empty,
                        TimeFrameInfoName = frame is not null ? $"{frame.TimeBegin.ToString("hh:mm:ss")} - {frame.TimeEnd.ToString("hh:mm:ss")}" : string.Empty
                    });
                }
                else
                {
                    _mailService.SendMail("bphn.email.approval-booking",
                        new ApprovalBookingParameter
                        {
                            ReceiverAddress = data.Email,
                            MailType = MailTypeEnum.APPROVALBOOKING,
                            ParameterType = typeof(ApprovalBookingParameter),
                            BookingDate = data.BookingDate.ToString("dd/MM/yyyy"),
                            NameDetail = data.NameDetail,
                            PhoneNumber = data.PhoneNumber,
                            StadiumName = pitch?.Name ?? string.Empty,
                            TimeFrameInfo = frame is not null ? $"{frame.TimeBegin.ToString("hh:mm:ss")} - {frame.TimeEnd.ToString("hh:mm:ss")}" : string.Empty,
                            Price = frame?.Price.ToString() ?? string.Empty,
                            MatchDate = matchDate ?? string.Empty
                        });

                    await _notificationService.Insert<Booking>(context, NotificationTypeEnum.APPROVALBOOKING, new Booking()
                    {
                        PhoneNumber = data.PhoneNumber,
                        NameDetail = data.NameDetail,
                        PitchName = pitch?.Name ?? string.Empty,
                        TimeFrameInfoName = frame is not null ? $"{frame.TimeBegin.ToString("hh:mm:ss")} - {frame.TimeEnd.ToString("hh:mm:ss")}" : string.Empty
                    });
                }

                _historyLogService.Write(Guid.NewGuid(), new HistoryLog
                {
                    ActionType = ActionEnum.UPDATE,
                    Entity = EntityEnum.BOOKING.ToString(),
                    Data = new HistoryLogDescription
                    {
                        ModelId = oldData.Id,
                        OldData = JsonConvert.SerializeObject(oldData),
                        NewData = JsonConvert.SerializeObject(data)
                    }
                }, context);
            }

            return new ServiceResultModel
            {
                Success = true,
                Data = updateResult
            };
        }

        private async Task<List<Booking>> GetAllTimeFrameInfoCanBookInADay(string accountId)
        {
            var lstBooking = new List<Booking>();
            var lstPitch = await _pitchRepository.GetPaging(1, int.MaxValue, new List<WhereCondition>()
            {
                new WhereCondition
                {
                    Column = "ManagerId",
                    Operator = "=",
                    Value = accountId
                },
                new WhereCondition
                {
                    Column = "Status",
                    Operator = "=",
                    Value = ActiveStatusEnum.ACTIVE.ToString()
                }
            });
            var now = DateTime.Now;
            var lstFrameInfo = await _timeFrameInfoRepository.GetByListPitchId(lstPitch.Select(item => item.Id).ToList());
            var dicFrame = new Dictionary<Guid, List<TimeFrameInfo>>();
            foreach (var frame in lstFrameInfo)
            {
                frame.TimeBegin = new DateTime(now.Year, now.Month, now.Day, frame.TimeBegin.Hour, frame.TimeBegin.Minute, 0);
                frame.TimeEnd = new DateTime(now.Year, now.Month, now.Day, frame.TimeEnd.Hour, frame.TimeEnd.Minute, 0);
                if (dicFrame.ContainsKey(frame.PitchId))
                {
                    var currentFrame = dicFrame[frame.PitchId];
                    currentFrame.Add(frame);
                    dicFrame[frame.PitchId] = currentFrame;
                }
                else
                {
                    dicFrame.Add(frame.PitchId, new List<TimeFrameInfo>() { frame });
                }
            }
            lstPitch = lstPitch.Select(item =>
            {
                item.TimeFrameInfos = dicFrame.ContainsKey(item.Id) ? dicFrame[item.Id] : new List<TimeFrameInfo>();
                return item;
            }).ToList();
            if (lstPitch is not null)
            {
                lstPitch = lstPitch.Where(item => item.Status == ActiveStatusEnum.ACTIVE.ToString()).ToList();
                for (int i = 0; i < lstPitch.Count; i++)
                {
                    var lstTimeFrameInfo = lstPitch[i].TimeFrameInfos;
                    var lstNameDetail = lstPitch[i].NameDetails.Split(";").ToList();

                    for (int j = 0; j < lstTimeFrameInfo.Count; j++)
                    {
                        for (int k = 0; k < lstNameDetail.Count; k++)
                        {
                            lstBooking.Add(new Booking()
                            {
                                Pitch = lstPitch[i],
                                PitchId = lstPitch[i].Id,
                                TimeFrameInfo = lstTimeFrameInfo[j],
                                TimeFrameInfoId = lstTimeFrameInfo[j].Id,
                                NameDetail = lstNameDetail[k]
                            });
                        }
                    }
                }
            }
            return lstBooking;
        }

        private async Task<HashSet<Tuple<Guid?, Guid?, string, DateTime>>> GetDictionaryBookedByDate(Guid accountId, DateTime startDate, DateTime endDate)
        {
            var lstBookedDetail = await _bookingDetailRepository.GetInRangeDate(accountId, startDate, endDate);
            var lstBookingId = string.Join(";", (new HashSet<Guid>(lstBookedDetail.Select(item => item.BookingId))).ToArray());
            var lstBooked = await _bookingRepository.GetById(lstBookingId);

            var hashSetResult = new HashSet<Tuple<Guid?, Guid?, string, DateTime>>();
            for (int i = 0; i < lstBooked.Count; i++)
            {
                var booked = lstBooked[i];
                var bookingDetails = lstBookedDetail.Where(item => item.BookingId == booked.Id).ToList();
                for (int j = 0; j < bookingDetails.Count; j++)
                {
                    hashSetResult.Add(new Tuple<Guid?, Guid?, string, DateTime>(booked.PitchId, booked.TimeFrameInfoId, booked.NameDetail, bookingDetails[j].MatchDate));
                }
            }
            return hashSetResult;
        }
    }
}
