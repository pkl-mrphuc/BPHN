using BPHN.ModelLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.DataLayer.IRepositories
{
    public interface IBookingDetailRepository
    {
        Task<List<BookingDetail>> GetInRangeDate(Guid accountId, DateTime startDate, DateTime endDate);
        Task<bool> Cancel(string id);
    }
}
