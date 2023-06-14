using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.ImpServices
{
    public class BookingService : BaseService, IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IContextService _contextService;
        private readonly IBookingDetailService _bookingDetailService;
        private readonly IHistoryLogService _historyLogService;
        private readonly IPitchService _pitchService;
        private readonly IBookingDetailRepository _bookingDetailRepository;
        public BookingService(IBookingRepository bookingRepository,
            IContextService contextService,
            IBookingDetailService bookingDetailService,
            IHistoryLogService historyLogService,
            IPitchService pitchService,
            IBookingDetailRepository bookingDetailRepository)
        {
            _bookingRepository = bookingRepository;
            _contextService = contextService;
            _bookingDetailService = bookingDetailService;
            _historyLogService = historyLogService;
            _pitchService = pitchService;
            _bookingDetailRepository = bookingDetailRepository;
        }

        public async Task<ServiceResultModel> CheckFreeTimeFrame(Booking data)
        {
            if (data.IsRecurring)
            {
                var matchDatesResult = _bookingDetailService.GetMatchDatesByWeekendays(data.StartDate, data.EndDate, data.Weekendays.Value);
                data.BookingDetails = matchDatesResult == null || matchDatesResult.Data == null ? new List<BookingDetail>() : (List<BookingDetail>)matchDatesResult.Data;
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

        public async Task<ServiceResultModel> Find(Booking data)
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

            var lstBooking = await GetAllTimeFrameInfoCanBookInADay(context.Id.ToString());

            if (lstBooking.Count > 0)
            {
                var now = DateTime.Now;
                data.EndDate = new DateTime(now.Year, 12, 31, 23, 59, 59);
                var dicMatchDate = await GetDictionaryBookedByDate(context.Id, data.StartDate, data.EndDate);

                if (data.IsRecurring)
                {
                    for (int i = 0; i <= 6; i++)
                    {
                        var serviceResult = _bookingDetailService.GetMatchDatesByWeekendays(data.StartDate, data.EndDate, i);
                        var lstDateByWeekendays = serviceResult == null || serviceResult.Data == null ? new List<BookingDetail>() : (List<BookingDetail>)serviceResult.Data;
                    }
                }
            }

            throw new Exception("");
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

            if (pageIndex < 0) pageIndex = 1;
            if (pageSize > 100 || pageSize <= 0) pageSize = 50;

            var result = await _bookingRepository.GetCountPaging(pageIndex, pageSize, context.Id, txtSearch);
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

            if (pageIndex < 0) pageIndex = 1;
            if (pageSize > 100 || pageSize <= 0) pageSize = 50;

            var lstBooking = await _bookingRepository.GetPaging(pageIndex, pageSize, context.Id, txtSearch, hasBookingDetail);
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

            bool insertResult = await _bookingRepository.Insert(data);
            if (insertResult)
            {
                Thread thread = new Thread(delegate ()
                {
                    _historyLogService.Write(new HistoryLog()
                    {
                        Actor = context.UserName,
                        ActorId = context.Id,
                        ActionType = ActionEnum.INSERT,
                        Description = "thông tin đặt sân"
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

        private async Task<List<Booking>> GetAllTimeFrameInfoCanBookInADay(string accountId)
        {
            var lstBooking = new List<Booking>();
            var lstPitch = (await _pitchService.GetPaging(1, int.MaxValue, string.Empty, accountId, true)).Data as List<Pitch>;
            if (lstPitch != null)
            {
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

        private async Task<Dictionary<string, List<Booking>>> GetDictionaryBookedByDate(Guid accountId, DateTime startDate, DateTime endDate)
        {
            List<BookingDetail> lstBookedDetail = await _bookingDetailRepository.GetInRangeDate(accountId, startDate, endDate);
            var lstBookingId = string.Join(";", (new HashSet<Guid>(lstBookedDetail.Select(item => item.BookingId))).ToArray());
            List<Booking> lstBooked = await _bookingRepository.GetById(lstBookingId);
            var dicBooked = new Dictionary<Guid, Booking>();
            for (int i = 0; i < lstBooked.Count; i++)
            {
                dicBooked.Add(lstBooked[i].Id, lstBooked[i]);
            }

            var dicMatchDate = new Dictionary<string, List<Booking>>();
            for (int i = 0; i < lstBookedDetail.Count; i++)
            {
                var item = lstBookedDetail[i];
                var key = item.MatchDate.ToString("dd/MM/yyyy");

                var value = new List<Booking>();
                if (dicMatchDate.ContainsKey(key))
                {
                    value = dicMatchDate[key];
                }
                else
                {
                    dicMatchDate.Add(key, value);
                }

                if (dicBooked.ContainsKey(item.BookingId))
                {
                    value.Add(dicBooked[item.BookingId]);
                }

                dicMatchDate[key] = value;
            }

            return dicMatchDate;
        }
    }
}
