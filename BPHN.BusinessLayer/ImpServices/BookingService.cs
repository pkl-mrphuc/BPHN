using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.ImpRepositories;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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
        public BookingService(
            IServiceProvider serviceProvider,
            IOptions<AppSettings> appSettings,
            IBookingRepository bookingRepository,
            IHistoryLogService historyLogService,
            IPitchRepository pitchRepository,
            IBookingDetailRepository bookingDetailRepository,
            IBookingDetailService bookingDetailService,
            INotificationService notificationService,
            ITimeFrameInfoRepository timeFrameInfoRepository) : base(serviceProvider, appSettings)
        {
            _bookingRepository = bookingRepository;
            _historyLogService = historyLogService;
            _bookingDetailRepository = bookingDetailRepository;
            _pitchRepository = pitchRepository;
            _bookingDetailService = bookingDetailService;
            _timeFrameInfoRepository = timeFrameInfoRepository;
            _notificationService = notificationService;
        }

        public async Task<ServiceResultModel> CheckFreeTimeFrame(Booking data)
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

            var hasPermissionAdd = await IsValidPermission(context.Id, FunctionTypeEnum.ADD_BOOKING);
            var hasPermissionEdit = await IsValidPermission(context.Id, FunctionTypeEnum.EDIT_BOOKING);
            if (!hasPermissionAdd && !hasPermissionEdit)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = "Bạn không có quyền thực hiện chức năng này"
                };
            }

            if (data.IsRecurring)
            {
                data.BookingDetails = _bookingDetailService.GetMatchDatesByWeekendays(data.StartDate, data.EndDate, data.Weekendays ?? (int)DayOfWeek.Monday);
            }
            else
            {
                data.BookingDetails = new List<BookingDetail>() {
                    new BookingDetail()
                    {
                        MatchDate = data.StartDate
                    }
                };
            }

            return new ServiceResultModel()
            {
                Success = await _bookingRepository.CheckFreeTimeFrame(data)
            };
        }

        public async Task<ServiceResultModel> FindBlank(Booking data)
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

            var hasPermissionAdd = await IsValidPermission(context.Id, FunctionTypeEnum.ADD_BOOKING);
            var hasPermissionEdit = await IsValidPermission(context.Id, FunctionTypeEnum.EDIT_BOOKING);
            if (!hasPermissionAdd && !hasPermissionEdit)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = "Bạn không có quyền thực hiện chức năng này"
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
                        var lstDateByWeekendays = _bookingDetailService.GetMatchDatesByWeekendays(data.StartDate, data.EndDate, i);
                        for (int j = 0; j < lstTimeFramesInADay.Count; j++)
                        {
                            var clone = (Booking)lstTimeFramesInADay[j].Clone();
                            clone.Id = Guid.NewGuid();
                            clone.IsRecurring = true;
                            clone.BookingDate = DateTime.Now;
                            clone.StartDate = data.StartDate;
                            clone.EndDate = data.EndDate;
                            clone.PitchName = clone.Pitch == null ? string.Empty : clone.Pitch.Name;
                            clone.TimeFrameInfoName = clone.TimeFrameInfo == null ? string.Empty : clone.TimeFrameInfo.Name;
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
                            clone.PitchName = clone.Pitch == null ? string.Empty : clone.Pitch.Name;
                            clone.TimeFrameInfoName = clone.TimeFrameInfo == null ? string.Empty : clone.TimeFrameInfo.Name;
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
                                                        a.BookingDetails.Where(c => setMatchDate.Contains(c.MatchDate.ToString("dd/MM/yyyy"))).FirstOrDefault() != null
                                                    ).FirstOrDefault() != null ? true : false;

                    if (!isConflict)
                    {
                        lstResult.Add(a);
                    }
                }
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = lstResult
            };
        }

        public async Task<ServiceResultModel> GetCountPaging(int pageIndex, int pageSize, string txtSearch)
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

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.VIEW_LIST_BOOKING);
            if (!hasPermission)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = "Bạn không có quyền thực hiện chức năng này"
                };
            }

            if (pageIndex < 0) pageIndex = 1;
            if (pageSize > 100 || pageSize <= 0) pageSize = 50;

            var result = await _bookingRepository.GetCountPaging(pageIndex, pageSize, context.RelationIds.ToArray(), txtSearch);
            return new ServiceResultModel()
            {
                Success = true,
                Data = result
            };
        }

        public async Task<ServiceResultModel> GetInstance(string id)
        {
            var data = new Booking();

            if (string.IsNullOrEmpty(id))
            {
                data.Id = Guid.NewGuid();
                data.BookingDate = DateTime.Now;
                data.StartDate = DateTime.Now;
                data.EndDate = DateTime.Now;
                data.IsRecurring = false;
                data.Weekendays = (int)DateTime.Now.DayOfWeek;
            }
            else
            {
                data = (await _bookingRepository.GetById(id)).FirstOrDefault();
                if (data == null)
                {
                    return new ServiceResultModel()
                    {
                        Success = false,
                        ErrorCode = ErrorCodes.NOT_EXISTS,
                        Message = "Không lấy được thông tin booking. Vui lòng kiểm tra lại"
                    };
                }
            }
            return new ServiceResultModel()
            {
                Success = true,
                Data = data
            };
        }

        public async Task<ServiceResultModel> GetPaging(int pageIndex, int pageSize, string txtSearch, bool hasBookingDetail = false)
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

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.VIEW_LIST_BOOKING);
            if (!hasPermission)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = "Bạn không có quyền thực hiện chức năng này"
                };
            }

            if (pageIndex < 0) pageIndex = 1;
            if (pageSize > 100 || pageSize <= 0) pageSize = 50;

            var lstBooking = await _bookingRepository.GetPaging(pageIndex, pageSize, context.RelationIds.ToArray(), txtSearch, hasBookingDetail);
            return new ServiceResultModel()
            {
                Success = true,
                Data = lstBooking
            };
        }

        public async Task<ServiceResultModel> Insert(Booking data)
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

            var hasPermission = await IsValidPermission(context.Id, FunctionTypeEnum.ADD_BOOKING);
            if (!hasPermission)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.INVALID_ROLE,
                    Message = "Bạn không có quyền thực hiện chức năng này"
                };
            }

            var isValid = ValidateModelByAttribute(data, new List<string>());
            if (!isValid)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không được để trống"
                };
            }

            if ((data.IsRecurring && data.StartDate.Date.Equals(data.EndDate.Date)) ||
                (data.IsRecurring && (!data.Weekendays.HasValue || (data.Weekendays.HasValue && data.Weekendays.Value < 0) || (data.Weekendays.HasValue && data.Weekendays.Value > 6))) ||
                data.EndDate.Date < DateTime.Now.Date)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NO_INTEGRITY,
                    Message = "Dữ liệu đầu vào không hợp lệ"
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
                var notification = _notificationService.Insert<Booking>(context, NotificationTypeEnum.ADD_BOOKING, data);
                Thread thread = new Thread(delegate ()
                {
                    var historyLogId = Guid.NewGuid();
                    _historyLogService.Write(new HistoryLog()
                    {
                        Id = historyLogId,
                        IPAddress = context.IPAddress,
                        Actor = context.UserName,
                        ActorId = context.Id,
                        ActionType = ActionEnum.INSERT,
                        Entity = "Thông tin đặt sân bóng",
                        Description = BuildLinkDescription(historyLogId),
                        Data = new HistoryLogDescription()
                        {
                            ModelId = data.Id,
                            NewData = JsonConvert.SerializeObject(data)
                        }
                    }, context);
                });
                thread.Start();
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = insertResult
            };
        }

        public async Task<ServiceResultModel> InsertBookingRequest(BookingRequest data)
        {
            var fakeContext = new Account()
            {
                FullName = data.PhoneNumber,
                UserName = data.PhoneNumber,
                IPAddress = _contextService.GetIPAddress()
            };

            var isValid = ValidateModelByAttribute(data, new List<string>());
            if (!isValid)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không được để trống"
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
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EXISTED,
                    Message = "Khung giờ này đã được đặt trước"
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

            booking.BookingDetails = new List<BookingDetail>()
            {
                new BookingDetail()
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    CreatedBy = fakeContext.UserName,
                    ModifiedBy = fakeContext.UserName,
                    ModifiedDate = DateTime.Now,
                    BookingId = data.Id,
                    Status = BookingStatusEnum.PENDING.ToString(),
                    MatchDate = data.StartDate,
                    Weekendays = (int)data.StartDate.DayOfWeek
                }
            };

            var insertResult = await _bookingRepository.Insert(booking);
            if (insertResult)
            {
                _notificationService.Insert<Booking>(fakeContext, NotificationTypeEnum.ADD_BOOKING, booking);
                Thread thread = new Thread(delegate ()
                {
                    var historyLogId = Guid.NewGuid();
                    _historyLogService.Write(new HistoryLog()
                    {
                        Id = historyLogId,
                        IPAddress = fakeContext.IPAddress,
                        Actor = fakeContext.FullName,
                        ActorId = data.AccountId,
                        ActionType = ActionEnum.INSERT,
                        Entity = "Thông tin đặt sân bóng",
                        Description = BuildLinkDescription(historyLogId),
                        Data = new HistoryLogDescription()
                        {
                            ModelId = data.Id,
                            NewData = JsonConvert.SerializeObject(data)
                        }
                    }, fakeContext);
                });
                thread.Start();
            }

            return new ServiceResultModel()
            {
                Success = true,
                Data = insertResult
            };
        }

        private async Task<List<Booking>> GetAllTimeFrameInfoCanBookInADay(string accountId)
        {
            var lstBooking = new List<Booking>();
            var lstPitch = await _pitchRepository.GetPaging(1, int.MaxValue, new List<WhereCondition>()
            {
                new WhereCondition()
                {
                    Column = "ManagerId",
                    Operator = "=",
                    Value = accountId
                },
                new WhereCondition()
                {
                    Column = "Status",
                    Operator = "=",
                    Value = "ACTIVE"
                }
            });
            var now = DateTime.Now;
            lstPitch = lstPitch.Select(item =>
            {
                item.TimeFrameInfos = _timeFrameInfoRepository.GetByPitchId(item.Id).Result.Select(item =>
                {
                    item.TimeBegin = new DateTime(now.Year, now.Month, now.Day, item.TimeBegin.Hour, item.TimeBegin.Minute, 0);
                    item.TimeEnd = new DateTime(now.Year, now.Month, now.Day, item.TimeEnd.Hour, item.TimeEnd.Minute, 0);
                    return item;
                }).ToList();
                return item;
            }).ToList();
            if (lstPitch != null)
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
                var bookingDetetails = lstBookedDetail.Where(item => item.BookingId == booked.Id).ToList();
                for (int j = 0; j < bookingDetetails.Count; j++)
                {
                    hashSetResult.Add(new Tuple<Guid?, Guid?, string, DateTime>(booked.PitchId, booked.TimeFrameInfoId, booked.NameDetail, bookingDetetails[j].MatchDate));
                }
            }
            return hashSetResult;
        }
    }
}
