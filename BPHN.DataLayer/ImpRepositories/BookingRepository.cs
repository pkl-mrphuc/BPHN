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
    public class BookingRepository : BaseRepository, IBookingRepository
    {
        public BookingRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public Booking? GetById(string id)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                dic.Add("@id", id);
                return connection.QueryFirstOrDefault<Booking>("select * from bookings where Id = @id", dic);
            }
        }
    }
}
