using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Microsoft.IdentityModel.Tokens;
using MySqlX.XDevAPI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.ImpServices
{
    public class BookingService : BaseService, IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IContextService _contextService;
        private readonly IBookingDetailService _bookingDetailService;
        private readonly IHistoryLogService _historyLogService;
        public BookingService(IBookingRepository bookingRepository,
            IContextService contextService,
            IBookingDetailService bookingDetailService, 
            IHistoryLogService historyLogService)
        {
            _bookingRepository = bookingRepository;
            _contextService = contextService;
            _bookingDetailService = bookingDetailService;
            _historyLogService = historyLogService;
        }

        public ServiceResultModel CheckFreeTimeFrame(Booking data)
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
                Success = _bookingRepository.CheckFreeTimeFrame(data),
                Data = null
            };
        }

        public ServiceResultModel GetInstance(string id)
        {
            var data = new Booking();

            if(string.IsNullOrEmpty(id))
            {
                data.Id = Guid.NewGuid();
                data.BookingDate = DateTime.Now;
                data.StartDate = DateTime.Now;
                data.EndDate = DateTime.Now;
                data.IsRecurring = false;
            }
            else
            {
                data = _bookingRepository.GetById(id);
                if(data == null)
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

        public ServiceResultModel Insert(Booking data)
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
            if(!isValid)
            {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.EMPTY_INPUT,
                    Message = "Dữ liệu đầu vào không được để trống"
                };
            }

            if((data.IsRecurring && data.StartDate.Date.Equals(data.EndDate.Date)) ||
                (data.IsRecurring && (!data.Weekendays.HasValue || (data.Weekendays.HasValue && data.Weekendays.Value < 0) || (data.Weekendays.HasValue && data.Weekendays.Value > 6))) ||
                data.EndDate.Date < DateTime.Now.Date) {
                return new ServiceResultModel()
                {
                    Success = false,
                    ErrorCode = ErrorCodes.NO_INTEGRITY,
                    Message = "Dữ liệu đầu vào không hợp lệ"
                };
            }

            var checkFreeServiceResult = CheckFreeTimeFrame(data);
            if (!checkFreeServiceResult.Success)
            {
                return checkFreeServiceResult;
            }

            data.Id = data.Id.Equals(Guid.Empty) ? Guid.NewGuid() : data.Id;
            data.BookingDate = DateTime.Now;
            data.Status = BookingStatusEnum.SUCCESS.ToString();
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
                return item;
            }).ToList();

            bool insertResult = _bookingRepository.Insert(data);
            if(insertResult)
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
    }
}
