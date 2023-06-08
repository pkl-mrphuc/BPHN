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
        ServiceResultModel GetInstance(string id);
        ServiceResultModel Insert(Booking data);
        ServiceResultModel CheckFreeTimeFrame(Booking data);
        ServiceResultModel GetPaging(int pageIndex, int pageSize, string txtSearch, bool hasBookingDetail = false);
        ServiceResultModel GetCountPaging(int pageIndex, int pageSize, string txtSearch);
    }
}
