using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
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
        Task<List<CalendarEvent>> GetByDate(string date, Guid[] relationIds);
        Task<bool> UpdateMatch(CalendarEvent eventInfo);
        Task<List<CalendarEvent>> GetByRangeDate(string startDate, string endDate, string pitchId, string nameDetail);
        Task<List<BookingDetail>> GetByBookingId(Guid bookingId);
        Task<BookingDetail?> GetById(string id);
    }
}
