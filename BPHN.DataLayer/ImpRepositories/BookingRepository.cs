using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Dapper;
using Microsoft.Extensions.Options;

namespace BPHN.DataLayer.ImpRepositories
{
    public class BookingRepository : BaseRepository, IBookingRepository
    {
        public BookingRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {
        }

        public async Task<bool> CheckFreeTimeFrame(Booking data)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                dic.Add("@pitchId", data.PitchId);
                dic.Add("@nameDetail", data.NameDetail);
                dic.Add("@timeFrameInfoId", data.TimeFrameInfoId);
                
                var lstParam = new List<string>();
                for (int i = 0; i < data.BookingDetails.Count; i++)
                {
                    var param = $"@date{i}";
                    dic.Add(param, $"{data.BookingDetails[i].MatchDate.ToString("yyyy-MM-dd")}");
                    lstParam.Add(param);
                }
                string where = string.Join(",", lstParam.ToArray());
                var query = $@"select bd.* from booking_details bd 
                                inner join bookings b on b.Id = bd.BookingId 
                                where b.PitchId = @pitchId and b.NameDetail = @nameDetail and b.TimeFrameInfoId = @timeFrameInfoId and bd.MatchDate in ({where})";
                

                var lstBooking = await connection.QueryFirstOrDefaultAsync<BookingDetail>(query, dic);
                return lstBooking != null ? false : true;
            }
        }

        public async Task<List<Booking>> GetById(string id)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                var lstId = id.Split(";").ToList();
                var lstParam = new List<string>();  
                for(int i = 0; i < lstId.Count; i++)
                {
                    var param = $"@id{i}";
                    dic.Add(param, lstId[i]);
                    lstParam.Add(param);
                }
                string where = string.Join(",", lstParam.ToArray());
                var lstBooking = (await connection.QueryAsync<Booking>($"select * from bookings where Id in ({where})", dic)).ToList();
                return lstBooking;
            }
        }

        public async Task<object> GetCountPaging(int pageIndex, int pageSize, Guid accountId, string txtSearch)
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
                int totalRecord = await connection.QuerySingleAsync<int>(countQuery, dic);
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

        public async Task<List<Booking>> GetPaging(int pageIndex, int pageSize, Guid accountId, string txtSearch, bool hasBookingDetail = false)
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
                                                                inner join time_frame_infos tfi on p.Id = tfi.PitchId and tfi.Id = bs.TimeFrameInfoId order by bs.BookingDate desc
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
                int totalRecord = await connection.QuerySingleAsync<int>(countQuery, dic);
                int totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                if (pageIndex > totalPage)
                {
                    pageIndex = 1;
                }
                int offSet = (pageIndex - 1) * pageSize;
                dic.Add("@offSize", offSet);
                dic.Add("@pageSize", pageSize);

                var lstBooking = (await connection.QueryAsync<Booking>(query, dic)).ToList();
                if(hasBookingDetail)
                {
                    
                    var lstBookingId = new List<string>();
                    dic = new Dictionary<string, object>();
                    for (int i = 0; i < lstBooking.Count; i++)
                    {
                        dic.Add($"@where{i}", lstBooking[i].Id);
                        lstBookingId.Add($"@where{i}");
                    }

                    string bookingIds = string.Join(",", lstBookingId.ToArray());

                    if (lstBooking.Count > 0)
                    {
                        string queryDetail = $"select * from booking_details where 1 = 1 and BookingId in ( {bookingIds} )";

                        var lstBookingDetail = (await connection.QueryAsync<BookingDetail>(queryDetail, dic)).ToList();
                        for (int i = 0; i < lstBooking.Count; i++)
                        {
                            var booking = lstBooking[i];
                            booking.BookingDetails = lstBookingDetail.Where(item => item.BookingId == booking.Id)
                                                                    .Select(item =>
                                                                    {
                                                                        item.Weekendays = (int)item.MatchDate.DayOfWeek;
                                                                        return item;
                                                                    })
                                                                    .OrderBy(item => item.MatchDate)
                                                                    .ToList();
                        }
                    }
                }

                return lstBooking;
            }
        }

        public async Task<bool> Insert(Booking data)
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
                int affect = await connection.ExecuteAsync(query, dic, transaction);
                if (affect > 0)
                {
                    for (int i = 0; i < data.BookingDetails.Count; i++)
                    {
                        var item = data.BookingDetails[i];
                        query = @"insert into booking_details(Id, MatchDate, BookingId, Status, Deposite, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                                value (@id, @matchDate, @bookingId, @status, @deposite, @createdDate, @createdBy, @modifiedDate, @modifiedBy)";
                        dic = new Dictionary<string, object>();
                        dic.Add("@id", item.Id);
                        dic.Add("@matchDate", item.MatchDate);
                        dic.Add("@bookingId", item.BookingId);
                        dic.Add("@status", item.Status);
                        dic.Add("@deposite", item.Deposite);
                        dic.Add("@createdDate", item.CreatedDate);
                        dic.Add("@createdBy", item.CreatedBy);
                        dic.Add("@modifiedBy", item.ModifiedBy);
                        dic.Add("@modifiedDate", item.ModifiedDate);
                        affect = await connection.ExecuteAsync(query, dic, transaction);
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
