using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.IServices
{
    public interface IBookingService
    {
        Task<ServiceResultModel> GetInstance(string id);
        Task<ServiceResultModel> Insert(Booking data);
        Task<ServiceResultModel> CheckFreeTimeFrame(Booking data);
        Task<ServiceResultModel> GetPaging(int pageIndex, int pageSize, string txtSearch, bool hasBookingDetail = false);
        Task<ServiceResultModel> GetCountPaging(int pageIndex, int pageSize, string txtSearch);
    }
}
