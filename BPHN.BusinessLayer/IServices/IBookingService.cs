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
        Task<ServiceResultModel> GetPaging(int pageIndex, int pageSize, string txtSearch, bool hasBookingDetail = false);
        Task<ServiceResultModel> GetCountPaging(int pageIndex, int pageSize, string txtSearch);
        Task<ServiceResultModel> GetPagingV1(int pageIndex, int pageSize, string txtSearch, bool hasBookingDetail = false);
        Task<ServiceResultModel> GetCountPagingV1(int pageIndex, int pageSize, string txtSearch);
        Task<ServiceResultModel> FindBlank(Booking data);
    }
}
