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

        public bool CheckFreeTimeFrame(Booking data)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                dic.Add("@pitchId", data.PitchId);
                dic.Add("@nameDetail", data.NameDetail);
                dic.Add("@timeFrameInfoId", data.TimeFrameInfoId);
                dic.Add("@date1", data.BookingDetails[0].MatchDate.ToString("yyyy-MM-dd"));
                dic.Add("@date2", data.BookingDetails[data.BookingDetails.Count - 1].MatchDate.ToString("yyyy-MM-dd"));

                var query = $@"select * from bookings b 
                                inner join booking_details bd on b.Id = bd.BookingId 
                                where b.PitchId = @pitchId and b.NameDetail = @nameDetail and b.TimeFrameInfoId = @timeFrameInfoId and bd.MatchDate between @date1 and @date2";
                

                var lstBooking = connection.QueryFirstOrDefault<Booking>(query, dic);
                return lstBooking != null ? false : true;
            }
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

        public bool Insert(Booking data)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                dic.Add("@id", data.Id);
                dic.Add("@phoneNumber", data.PhoneNumber);
                dic.Add("@email", data.Email);
                dic.Add("@isRecurring", data.IsRecurring);
                dic.Add("@bookingDate", data.BookingDate);
                dic.Add("@startDate", data.StartDate);
                dic.Add("@endDate", data.EndDate);
                dic.Add("@weekendays", data.Weekendays);
                dic.Add("@status", data.Status);
                dic.Add("@timeFrameInfoId", data.TimeFrameInfoId);
                dic.Add("@pitchId", data.PitchId);
                dic.Add("@nameDetail", data.NameDetail);
                dic.Add("@createdDate", data.CreatedDate);
                dic.Add("@createdBy", data.CreatedBy);
                dic.Add("@modifiedBy", data.ModifiedBy);
                dic.Add("@modifiedDate", data.ModifiedDate);
                var transaction = connection.BeginTransaction();
                var query = @"insert into bookings(Id, PhoneNumber, Email, IsRecurring, BookingDate, StartDate, EndDate, Weekendays, Status, TimeFrameInfoId, PitchId, NameDetail, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                            value (@id, @phoneNumber, @email, @isRecurring, @bookingDate, @startDate, @endDate, @weekendays, @status, @timeFrameInfoId, @pitchId, @nameDetail, @createdDate, @createdBy, @modifiedDate, @modifiedBy)";
                int affect = connection.Execute(query, dic, transaction);
                if (affect > 0)
                {
                    for (int i = 0; i < data.BookingDetails.Count; i++)
                    {
                        var item = data.BookingDetails[i];
                        query = @"insert into booking_details(Id, MatchDate, BookingId, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                                value (@id, @matchDate, @bookingId, @createdDate, @createdBy, @modifiedDate, @modifiedBy)";
                        dic = new Dictionary<string, object>();
                        dic.Add("@id", item.Id);
                        dic.Add("@matchDate", item.MatchDate);
                        dic.Add("@bookingId", item.BookingId);
                        dic.Add("@createdDate", item.CreatedDate);
                        dic.Add("@createdBy", item.CreatedBy);
                        dic.Add("@modifiedBy", item.ModifiedBy);
                        dic.Add("@modifiedDate", item.ModifiedDate);
                        affect = connection.Execute(query, dic, transaction);
                        if(affect <= 0)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }

                    transaction.Commit();
                    return true;
                }

                return false;
            }
        }
    }
}
