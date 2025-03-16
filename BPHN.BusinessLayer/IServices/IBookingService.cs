using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;

namespace BPHN.BusinessLayer.IServices
{
    public interface IBookingService
    {
        Task<ServiceResultModel> GetInstance(string id);
        Task<ServiceResultModel> Insert(Booking data);
        Task<ServiceResultModel> Update(string id, BookingStatusEnum status);
        Task<ServiceResultModel> InsertBookingRequest(BookingRequest data);
        Task<ServiceResultModel> CheckFreeTimeFrame(Booking data);
        Task<ServiceResultModel> GetPaging(GetBookingPagingModel model);
        Task<ServiceResultModel> GetCountPaging(GetBookingPagingModel model);
        Task<ServiceResultModel> FindBlank(Booking data);
    }
}
