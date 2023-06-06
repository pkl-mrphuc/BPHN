using BPHN.BusinessLayer.IServices;
using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
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
        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
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
    }
}
