using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
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
                var dic = new Dictionary<string, object?>();
                dic.Add("@pitchId", data.PitchId);
                dic.Add("@nameDetail", data.NameDetail);
                dic.Add("@timeFrameInfoId", data.TimeFrameInfoId);
                dic.Add("@status0", BookingStatusEnum.SUCCESS.ToString());

                var lstParam = new List<string>();
                for (int i = 0; i < data.BookingDetails.Count; i++)
                {
                    var param = $"@date{i}";
                    dic.Add(param, $"{data.BookingDetails[i].MatchDate.ToString("yyyy-MM-dd")}");
                    lstParam.Add(param);
                }
                var where = string.Join(",", lstParam.ToArray());
                var query = $@"select bd.* from booking_details bd 
                                inner join bookings b on b.Id = bd.BookingId 
                                where bd.Status in (@status0) and b.PitchId = @pitchId and b.NameDetail = @nameDetail and b.TimeFrameInfoId = @timeFrameInfoId and bd.MatchDate in ({where})";


                var lstBooking = await connection.QueryFirstOrDefaultAsync<BookingDetail>(query, dic);
                return lstBooking != null ? false : true;
            }
        }

        public async Task<List<Booking>> GetByIds(string ids)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                var lstId = ids.Split(";").ToList();
                var lstParam = new List<string>();
                for (int i = 0; i < lstId.Count; i++)
                {
                    var param = $"@id{i}";
                    dic.Add(param, lstId[i]);
                    lstParam.Add(param);
                }
                var where = string.Join(",", lstParam.ToArray());
                var lstBooking = await connection.QueryAsync<Booking>($"select * from bookings where Id in ({where})", dic);
                return lstBooking.ToList();
            }
        }

        public async Task<object> GetCountPaging(int pageIndex, int pageSize, Guid[] relationIds, string txtSearch)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                var countQuery = @"select distinct count(*) from (
						                                                    select * from bookings where AccountId in @accountId and PhoneNumber like @txtSearch
                                                                            union 
                                                                            select * from bookings where AccountId in @accountId and Email like @txtSearch 
                                                                            union 
                                                                            select * from bookings where AccountId in @accountId and NameDetail like @txtSearch
                                                                            union 
                                                                            (select b.* from bookings b inner join pitchs p on b.PitchId = p.Id where b.AccountId in @accountId and p.Name like @txtSearch)
                                                                        ) as bs";

                dic.Add("@accountId", relationIds);
                dic.Add("@txtSearch", $"%{txtSearch}%");
                var totalRecord = await connection.QuerySingleAsync<int>(countQuery, dic);
                var totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                var totalRecordCurrentPage = 0;
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

        public async Task<List<Booking>> GetPaging(int pageIndex, int pageSize, Guid[] relationIds, string txtSearch, bool hasBookingDetail = false)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                var query = @"select distinct   bs.*, 
                                                p.Name as PitchName, 
                                                concat('Khung ', date_format(TimeBegin,'%H:%i'), ' - ', date_format(TimeEnd,'%H:%i')) as TimeFrameInfoName 
                                                from (
						                                    select * from bookings where AccountId in @accountId and PhoneNumber like @txtSearch
                                                            union 
                                                            select * from bookings where AccountId in @accountId and Email like @txtSearch 
                                                            union 
                                                            select * from bookings where AccountId in @accountId and NameDetail like @txtSearch
                                                            union 
                                                            (select b.* from bookings b inner join pitchs p on b.PitchId = p.Id where b.AccountId in @accountId and p.Name like @txtSearch)
                                                        ) as bs inner join pitchs p on bs.PitchId = p.Id
                                                                inner join time_frame_infos tfi on p.Id = tfi.PitchId and tfi.Id = bs.TimeFrameInfoId order by bs.BookingDate desc
                        limit @offSize, @pageSize";
                var countQuery = @"select distinct count(*) from (
						                                                    select * from bookings where AccountId in @accountId and PhoneNumber like @txtSearch
                                                                            union 
                                                                            select * from bookings where AccountId in @accountId and Email like @txtSearch 
                                                                            union 
                                                                            select * from bookings where AccountId in @accountId and NameDetail like @txtSearch
                                                                            union 
                                                                            (select b.* from bookings b inner join pitchs p on b.PitchId = p.Id where b.AccountId in @accountId and p.Name like @txtSearch)
                                                                        ) as bs";

                dic.Add("@accountId", relationIds);
                dic.Add("@txtSearch", $"%{txtSearch}%");
                var totalRecord = await connection.QuerySingleAsync<int>(countQuery, dic);
                var totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                if (pageIndex > totalPage)
                {
                    pageIndex = 1;
                }
                var offSet = (pageIndex - 1) * pageSize;
                dic.Add("@offSize", offSet);
                dic.Add("@pageSize", pageSize);

                var lstBooking = (await connection.QueryAsync<Booking>(query, dic)).ToList();
                if (hasBookingDetail)
                {

                    var lstBookingId = new List<string>();
                    dic = new Dictionary<string, object>();
                    for (int i = 0; i < lstBooking.Count; i++)
                    {
                        dic.Add($"@where{i}", lstBooking[i].Id);
                        lstBookingId.Add($"@where{i}");
                    }

                    var bookingIds = string.Join(",", lstBookingId.ToArray());

                    if (lstBooking.Count > 0)
                    {
                        var queryDetail = $"select * from booking_details where 1 = 1 and BookingId in ( {bookingIds} )";

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
                var dic = new Dictionary<string, object?>();
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
                    dic = new Dictionary<string, object?>();
                    var rows = new List<string>();
                    for (int i = 0; i < data.BookingDetails.Count; i++)
                    {
                        var item = data.BookingDetails[i];
                        dic.Add($"@id{i}", item.Id);
                        dic.Add($"@matchDate{i}", item.MatchDate);
                        dic.Add($"@bookingId{i}", item.BookingId);
                        dic.Add($"@status{i}", item.Status);
                        dic.Add($"@deposite{i}", item.Deposite);
                        dic.Add($"@createdDate{i}", item.CreatedDate);
                        dic.Add($"@createdBy{i}", item.CreatedBy);
                        dic.Add($"@modifiedBy{i}", item.ModifiedBy);
                        dic.Add($"@modifiedDate{i}", item.ModifiedDate);
                        dic.Add($"@teamA{i}", item.TeamA);
                        dic.Add($"@note{i}", item.Note);
                        rows.Add($"(@id{i}, @matchDate{i}, @bookingId{i}, @status{i}, @deposite{i}, @teamA{i}, @note{i}, @createdDate{i}, @createdBy{i}, @modifiedDate{i}, @modifiedBy{i})");
                    }

                    query = $"insert into booking_details(Id, MatchDate, BookingId, Status, Deposite, TeamA, Note, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy) values {string.Join(",", rows)}";
                    affect = await connection.ExecuteAsync(query, dic, transaction);
                    if (affect <= 0)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    transaction.Commit();
                    return true;
                }

                return false;
            }
        }

        public async Task<bool> Update(Booking data)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object?>();
                dic.Add("@id", data.Id);
                dic.Add("@status", data.Status);
                dic.Add("@modifiedBy", data.ModifiedBy);
                dic.Add("@modifiedDate", data.ModifiedDate);
                var transaction = connection.BeginTransaction();
                var query = @"update bookings set Status = @status, ModifiedDate = @modifiedDate, ModifiedBy = @modifiedBy where Id = @id";
                int affect = await connection.ExecuteAsync(query, dic, transaction);
                if (affect > 0)
                {
                    for (int i = 0; i < data.BookingDetails.Count; i++)
                    {
                        var item = data.BookingDetails[i];
                        query = @"update booking_details set Status = @status, ModifiedDate = @modifiedDate, ModifiedBy = @modifiedBy where Id = @id";
                        dic = new Dictionary<string, object?>();
                        dic.Add("@id", item.Id);
                        dic.Add("@status", item.Status);
                        dic.Add("@modifiedBy", item.ModifiedBy);
                        dic.Add("@modifiedDate", item.ModifiedDate);
                        affect = await connection.ExecuteAsync(query, dic, transaction);
                        if (affect <= 0)
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

        public async Task<object> GetCountPagingV1(int pageIndex, int pageSize, Guid[] relationIds, string txtSearch)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                var countQuery = @"select distinct count(1) from booking_details bd 
                                                            inner join (
						                                                    select * from bookings where AccountId in @accountId and PhoneNumber like @txtSearch
                                                                            union 
                                                                            select * from bookings where AccountId in @accountId and Email like @txtSearch 
                                                                            union 
                                                                            select * from bookings where AccountId in @accountId and NameDetail like @txtSearch
                                                                            union 
                                                                            (select b.* from bookings b inner join pitchs p on b.PitchId = p.Id where b.AccountId in @accountId and p.Name like @txtSearch)
                                                                        ) as bs on bs.Id = bd.BookingId";

                dic.Add("@accountId", relationIds);
                dic.Add("@txtSearch", $"%{txtSearch}%");
                var totalRecord = await connection.QuerySingleAsync<int>(countQuery, dic);
                var totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                var totalRecordCurrentPage = 0;
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

        public async Task<List<BookingManager>> GetPagingV1(int pageIndex, int pageSize, Guid[] relationIds, string txtSearch, bool hasBookingDetail = false)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                var query = @"select distinct   bs.BookingCode as BookingCode,
                                                bs.PhoneNumber as PhoneNumber,
                                                bs.Email as Email,
                                                bs.IsRecurring as IsRecurring,
                                                bs.BookingDate as BookingDate,
                                                bs.StartDate as StartDate,
                                                bs.EndDate as EndDate,
                                                bs.Status as BookingStatus,
                                                (CASE
                                                    WHEN bd.Status = 'CANCEL' THEN 2
                                                    WHEN bd.Status = 'SUCCESS' THEN 1
                                                    ELSE 0
                                                END) as StatusId,
                                                bs.TimeFrameInfoId as TimeFrameInfoId,
                                                bs.PitchId as PitchId,
                                                bs.NameDetail as NameDetail,
                                                p.Name as PitchName, 
                                                concat('Khung ', date_format(TimeBegin,'%H:%i'), ' - ', date_format(TimeEnd,'%H:%i')) as TimeFrameInfoName,
                                                bd.Id as BookingDetailId,
                                                bd.MatchCode as MatchCode,
                                                bd.MatchDate as MatchDate,
                                                bd.Deposite as Deposite,
                                                bd.BookingId as BookingId,
                                                bd.Status as BookingDetailStatus
                                                from booking_details bd 
                                                inner join (
						                                    select * from bookings where AccountId in @accountId and PhoneNumber like @txtSearch
                                                            union 
                                                            select * from bookings where AccountId in @accountId and Email like @txtSearch 
                                                            union 
                                                            select * from bookings where AccountId in @accountId and NameDetail like @txtSearch
                                                            union 
                                                            (select b.* from bookings b inner join pitchs p on b.PitchId = p.Id where b.AccountId in @accountId and p.Name like @txtSearch)
                                                        ) as bs on bs.Id = bd.BookingId
                                                    inner join pitchs p on bs.PitchId = p.Id
                                                    inner join time_frame_infos tfi on p.Id = tfi.PitchId and tfi.Id = bs.TimeFrameInfoId order by StatusId, bd.MatchDate desc
                        limit @offSize, @pageSize";
                var countQuery = @"select distinct count(1) from booking_details bd 
                                                            inner join (
						                                                    select * from bookings where AccountId in @accountId and PhoneNumber like @txtSearch
                                                                            union 
                                                                            select * from bookings where AccountId in @accountId and Email like @txtSearch 
                                                                            union 
                                                                            select * from bookings where AccountId in @accountId and NameDetail like @txtSearch
                                                                            union 
                                                                            (select b.* from bookings b inner join pitchs p on b.PitchId = p.Id where b.AccountId in @accountId and p.Name like @txtSearch)
                                                                        ) as bs on bs.Id = bd.BookingId";

                dic.Add("@accountId", relationIds);
                dic.Add("@txtSearch", $"%{txtSearch}%");
                var totalRecord = await connection.QuerySingleAsync<int>(countQuery, dic);
                var totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                if (pageIndex > totalPage)
                {
                    pageIndex = 1;
                }
                var offSet = (pageIndex - 1) * pageSize;
                dic.Add("@offSize", offSet);
                dic.Add("@pageSize", pageSize);

                var lstBooking = (await connection.QueryAsync<BookingManager>(query, dic)).ToList();
                lstBooking = lstBooking.Select(item =>
                                        {
                                            item.Weekendays = (int)item.MatchDate.DayOfWeek;
                                            return item;
                                        }).ToList();
                return lstBooking;
            }
        }

        public async Task<Booking> GetById(string id)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<Booking>(Query.BOOKING__GET_BY_ID, new Dictionary<string, object>
                {
                    { "@id", id }
                });
            }
        }
    }
}
