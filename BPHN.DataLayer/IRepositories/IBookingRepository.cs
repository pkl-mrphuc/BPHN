using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.DataLayer.IRepositories
{
    public interface IBookingRepository
    {
        Task<Booking?> GetById(string id);
        Task<bool> CheckFreeTimeFrame(Booking data);
        Task<bool> Insert(Booking data);
        Task<object> GetCountPaging(int pageIndex, int pageSize, Guid accountId, string txtSearch);
        Task<List<Booking>> GetPaging(int pageIndex, int pageSize, Guid accountId, string txtSearch, bool hasBookingDetail = false);
    }
}
