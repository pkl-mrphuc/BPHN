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
                return lstBooking is null;
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
                        dic.Add($"@deposit{i}", item.Deposit);
                        dic.Add($"@createdDate{i}", item.CreatedDate);
                        dic.Add($"@createdBy{i}", item.CreatedBy);
                        dic.Add($"@modifiedBy{i}", item.ModifiedBy);
                        dic.Add($"@modifiedDate{i}", item.ModifiedDate);
                        dic.Add($"@teamA{i}", item.TeamA);
                        dic.Add($"@note{i}", item.Note);
                        rows.Add($"(@id{i}, @matchDate{i}, @bookingId{i}, @status{i}, @deposit{i}, @teamA{i}, @note{i}, @createdDate{i}, @createdBy{i}, @modifiedDate{i}, @modifiedBy{i})");
                    }

                    query = $"insert into booking_details(Id, MatchDate, BookingId, Status, Deposit, TeamA, Note, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy) values {string.Join(",", rows)}";
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

        public async Task<object> GetCountPaging(GetBookingPagingModel model)
        {
            var conditions = new List<WhereCondition>
            {
                new WhereCondition
                {
                    Column = "b.AccountId",
                    Operator = "=",
                    Value = model.AccountId
                }
            };
            if (!string.IsNullOrWhiteSpace(model.TxtSearch))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "b.PhoneNumber",
                    Operator = "like",
                    Value = $"%{model.TxtSearch}%"
                });
            }
            if (!string.IsNullOrWhiteSpace(model.Status))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "bd.Status",
                    Operator = "=",
                    Value = model.Status
                });
            }
            if (model.BookingDate.HasValue)
            {
                conditions.Add(new WhereCondition
                {
                    Column = "b.BookingDate",
                    Operator = ">=",
                    Value = model.BookingDate.Value.ToString("yyyy-MM-dd 00:00:00")
                });
                conditions.Add(new WhereCondition
                {
                    Column = "b.BookingDate",
                    Operator = "<=",
                    Value = model.BookingDate.Value.ToString("yyyy-MM-dd 23:59:59")
                });
            }
            if (model.MatchDate.HasValue)
            {
                conditions.Add(new WhereCondition
                {
                    Column = "bd.MatchDate",
                    Operator = ">=",
                    Value = model.MatchDate.Value.ToString("yyyy-MM-dd 00:00:00")
                });
                conditions.Add(new WhereCondition
                {
                    Column = "bd.MatchDate",
                    Operator = "<=",
                    Value = model.MatchDate.Value.ToString("yyyy-MM-dd 23:59:59")
                });
            }
            if (DepositStatusEnum.DEPOSITED.ToString().Equals(model.Deposit))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "bd.Deposit",
                    Operator = ">",
                    Value = 0
                });
            }
            if (DepositStatusEnum.NOTDEPOSIT.ToString().Equals(model.Deposit))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "bd.Deposit",
                    Operator = "=",
                    Value = 0
                });
            }
            if (model.PitchId.HasValue)
            {
                conditions.Add(new WhereCondition
                {
                    Column = "b.PitchId",
                    Operator = "=",
                    Value = model.PitchId.Value
                });
            }
            if (model.TimeFrameId.HasValue)
            {
                conditions.Add(new WhereCondition
                {
                    Column = "b.TimeFrameInfoId",
                    Operator = "=",
                    Value = model.TimeFrameId.Value
                });
            }
            if (!string.IsNullOrWhiteSpace(model.NameDetail))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "b.NameDetail",
                    Operator = "=",
                    Value = model.NameDetail
                });
            }

            var preQuery = @"select count(1) from booking_details bd 
                                    inner join bookings b on b.Id = bd.BookingId 
                                    inner join pitchs p on b.PitchId = p.Id";
            var where = BuildWhere(preQuery, conditions);
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var totalRecord = await connection.QuerySingleAsync<int>(where.query, where.param);
                var totalPage = totalRecord % model.PageSize == 0 ? totalRecord / model.PageSize : (totalRecord / model.PageSize) + 1;
                var totalRecordCurrentPage = 0;
                if (totalRecord > 0)
                {
                    if (model.PageIndex == totalPage)
                    {
                        totalRecordCurrentPage = totalRecord - ((model.PageIndex - 1) * model.PageSize);
                    }
                    else
                    {
                        totalRecordCurrentPage = model.PageSize;
                    }
                }
                return new { TotalPage = totalPage, TotalRecordCurrentPage = totalRecordCurrentPage, TotalAllRecords = totalRecord };
            }
        }

        public async Task<IEnumerable<BookingManager>> GetPaging(GetBookingPagingModel model)
        {
            var conditions = new List<WhereCondition>
            {
                new WhereCondition
                {
                    Column = "b.AccountId",
                    Operator = "=",
                    Value = model.AccountId
                }
            };
            if (!string.IsNullOrWhiteSpace(model.TxtSearch))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "b.PhoneNumber",
                    Operator = "like",
                    Value = $"%{model.TxtSearch}%"
                });
            }
            if (!string.IsNullOrWhiteSpace(model.Status))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "bd.Status",
                    Operator = "=",
                    Value = model.Status
                });
            }
            if (model.BookingDate.HasValue)
            {
                conditions.Add(new WhereCondition
                {
                    Column = "b.BookingDate",
                    Operator = ">=",
                    Value = model.BookingDate.Value.ToString("yyyy-MM-dd 00:00:00")
                });
                conditions.Add(new WhereCondition
                {
                    Column = "b.BookingDate",
                    Operator = "<=",
                    Value = model.BookingDate.Value.ToString("yyyy-MM-dd 23:59:59")
                });
            }
            if (model.MatchDate.HasValue)
            {
                conditions.Add(new WhereCondition
                {
                    Column = "bd.MatchDate",
                    Operator = ">=",
                    Value = model.MatchDate.Value.ToString("yyyy-MM-dd 00:00:00")
                });
                conditions.Add(new WhereCondition
                {
                    Column = "bd.MatchDate",
                    Operator = "<=",
                    Value = model.MatchDate.Value.ToString("yyyy-MM-dd 23:59:59")
                });
            }
            if (DepositStatusEnum.DEPOSITED.ToString().Equals(model.Deposit))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "bd.Deposit",
                    Operator = ">",
                    Value = 0
                });
            }
            if (DepositStatusEnum.NOTDEPOSIT.ToString().Equals(model.Deposit))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "bd.Deposit",
                    Operator = "=",
                    Value = 0
                });
            }
            if (model.PitchId.HasValue)
            {
                conditions.Add(new WhereCondition
                {
                    Column = "b.PitchId",
                    Operator = "=",
                    Value = model.PitchId.Value
                });
            }
            if (model.TimeFrameId.HasValue)
            {
                conditions.Add(new WhereCondition
                {
                    Column = "b.TimeFrameInfoId",
                    Operator = "=",
                    Value = model.TimeFrameId.Value
                });
            }
            if (!string.IsNullOrWhiteSpace(model.NameDetail))
            {
                conditions.Add(new WhereCondition
                {
                    Column = "b.NameDetail",
                    Operator = "=",
                    Value = model.NameDetail
                });
            }

            var preQuery = @"select distinct   
                                b.BookingCode as BookingCode,
                                b.PhoneNumber as PhoneNumber,
                                b.Email as Email,
                                b.IsRecurring as IsRecurring,
                                b.BookingDate as BookingDate,
                                b.StartDate as StartDate,
                                b.EndDate as EndDate,
                                b.Status as BookingStatus,
                                b.TimeFrameInfoId as TimeFrameInfoId,
                                b.PitchId as PitchId,
                                b.NameDetail as NameDetail,
                                p.Name as PitchName, 
                                concat(date_format(TimeBegin,'%H:%i:%s'), ' - ', date_format(TimeEnd,'%H:%i:%s')) as TimeFrameInfoName,
                                bd.Id as BookingDetailId,
                                bd.MatchCode as MatchCode,
                                bd.MatchDate as MatchDate,
                                bd.Deposit as Deposit,
                                bd.BookingId as BookingId,
                                bd.Status as BookingDetailStatus,
                                tfi.Price as Price,
                                (WEEKDAY(bd.MatchDate) + 1) % 7 as Weekendays
                            from booking_details bd  
                                inner join bookings b on b.Id = bd.BookingId
                                inner join pitchs p on b.PitchId = p.Id
                                inner join time_frame_infos tfi on tfi.Id = b.TimeFrameInfoId";
            var behindQuery = "limit @offSize, @pageSize";
            var where = BuildWhere(preQuery, conditions, behindQuery);
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = where.param;
                dic.Add("@offSize", (model.PageIndex - 1) * model.PageSize);
                dic.Add("@pageSize", model.PageSize);

                var lstBooking = (await connection.QueryAsync<BookingManager>(where.query, dic));
                return lstBooking ?? Enumerable.Empty<BookingManager>();
            }
        }

        public async Task<Booking> GetById(Guid id)
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
