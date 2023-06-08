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

        public object GetCountPaging(int pageIndex, int pageSize, Guid accountId, string txtSearch)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                string countQuery = @"select distinct count(*) from (
						                                                    select * from bookings where AccountId = @accountId and PhoneNumber like @txtSearch
                                                                            union 
                                                                            select * from bookings where AccountId = @accountId and Email like @txtSearch 
                                                                            union 
                                                                            select * from bookings where AccountId = @accountId and NameDetail like @txtSearch
                                                                            union 
                                                                            (select b.* from bookings b inner join pitchs p on b.PitchId = p.Id where b.AccountId = @accountId and p.Name like @txtSearch)
                                                                        ) as bs";

                dic.Add("@accountId", accountId);
                dic.Add("@txtSearch", $"%{txtSearch}%");
                int totalRecord = connection.QuerySingle<int>(countQuery, dic);
                int totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                int totalRecordCurrentPage = 0;
                if (totalRecord > 0)
                {
                    if (pageIndex == totalPage)
                    {
                        totalRecordCurrentPage = totalRecord - ((pageIndex - 1) * pageSize);
                    }
                    else
                    {
                        totalRecordCurrentPage = pageSize;
                    }
                }
                return new { TotalPage = totalPage, TotalRecordCurrentPage = totalRecordCurrentPage, TotalAllRecords = totalRecord };
            }
        }

        public List<Booking> GetPaging(int pageIndex, int pageSize, Guid accountId, string txtSearch, bool hasBookingDetail = false)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                var query = @"select distinct bs.*, p.Name as PitchName, tfi.Name as TimeFrameInfoName from (
						                                    select * from bookings where AccountId = @accountId and PhoneNumber like @txtSearch
                                                            union 
                                                            select * from bookings where AccountId = @accountId and Email like @txtSearch 
                                                            union 
                                                            select * from bookings where AccountId = @accountId and NameDetail like @txtSearch
                                                            union 
                                                            (select b.* from bookings b inner join pitchs p on b.PitchId = p.Id where b.AccountId = @accountId and p.Name like @txtSearch)
                                                        ) as bs inner join pitchs p on bs.PitchId = p.Id
                                                                inner join time_frame_infos tfi on p.Id = tfi.PitchId
                        limit @offSize, @pageSize";
                string countQuery = @"select distinct count(*) from (
						                                                    select * from bookings where AccountId = @accountId and PhoneNumber like @txtSearch
                                                                            union 
                                                                            select * from bookings where AccountId = @accountId and Email like @txtSearch 
                                                                            union 
                                                                            select * from bookings where AccountId = @accountId and NameDetail like @txtSearch
                                                                            union 
                                                                            (select b.* from bookings b inner join pitchs p on b.PitchId = p.Id where b.AccountId = @accountId and p.Name like @txtSearch)
                                                                        ) as bs";
                
                dic.Add("@accountId", accountId);
                dic.Add("@txtSearch", $"%{txtSearch}%");
                int totalRecord = connection.QuerySingle<int>(countQuery, dic);
                int totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                if (pageIndex > totalPage)
                {
                    pageIndex = 1;
                }
                int offSet = (pageIndex - 1) * pageSize;
                dic.Add("@offSize", offSet);
                dic.Add("@pageSize", pageSize);

                var lstBooking = connection.Query<Booking>(query, dic).ToList();
                if(hasBookingDetail)
                {
                    string bookingIds = string.Empty;
                    dic = new Dictionary<string, object>();
                    for (int i = 0; i < lstBooking.Count; i++)
                    {
                        dic.Add($"@where{i}", lstBooking[i].Id);
                        bookingIds += $" and @where{i} ";
                    }

                    if(lstBooking.Count > 0)
                    {
                        string queryDetail = $"select * from booking_details where 1 = 1 {bookingIds}";

                        var lstBookingDetail = connection.Query<BookingDetail>(queryDetail, dic).ToList();
                        for (int i = 0; i < lstBooking.Count; i++)
                        {
                            var booking = lstBooking[i];
                            booking.BookingDetails = lstBookingDetail.Where(item => item.BookingId == booking.Id).ToList();
                        }
                    }
                }

                return lstBooking;
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
                dic.Add("@accountId", data.AccountId);
                dic.Add("@createdDate", data.CreatedDate);
                dic.Add("@createdBy", data.CreatedBy);
                dic.Add("@modifiedBy", data.ModifiedBy);
                dic.Add("@modifiedDate", data.ModifiedDate);
                var transaction = connection.BeginTransaction();
                var query = @"insert into bookings(Id, PhoneNumber, Email, IsRecurring, BookingDate, StartDate, EndDate, Weekendays, Status, TimeFrameInfoId, PitchId, NameDetail, accountId, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                            value (@id, @phoneNumber, @email, @isRecurring, @bookingDate, @startDate, @endDate, @weekendays, @status, @timeFrameInfoId, @pitchId, @nameDetail, @accountId, @createdDate, @createdBy, @modifiedDate, @modifiedBy)";
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
