using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.DataLayer.ImpRepositories
{
    public class BookingDetailRepository : BaseRepository, IBookingDetailRepository
    {
        public BookingDetailRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public async Task<List<BookingDetail>> GetInRangeDate(Guid accountId, DateTime startDate, DateTime endDate)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                dic.Add("@accountId", accountId);
                dic.Add("@startDate", startDate);
                dic.Add("@endDate", endDate);
                var query = @"select bd.* from booking_details bd inner join bookings b on b.Id = bd.BookingId where b.AccountId = @accountId and bd.MatchDate between @startDate and @endDate";
                var data = await connection.QueryAsync<BookingDetail>(query, dic);
                return data.ToList();
            }
        }
    }
}
