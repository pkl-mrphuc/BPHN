using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Dapper;
using Microsoft.Extensions.Options;

namespace BPHN.DataLayer.ImpRepositories
{
    public class PitchRepository : BaseRepository, IPitchRepository
    {
        public PitchRepository(IOptions<AppSettings> appSetting) : base(appSetting)
        {
                
        }

        public async Task<List<Pitch>> GetAll(string accountId)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var lstPitch = await connection.QueryAsync<Pitch>(Query.PITCH__GET_ALL, new Dictionary<string, object>
                {
                    { "@accountId", accountId },
                    { "@status", ActiveStatusEnum.ACTIVE.ToString() },
                });
                return lstPitch?.ToList() ?? new List<Pitch>();
            }
        }

        public async Task<Pitch?> GetById(Guid id)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var pitch = await connection.QueryFirstOrDefaultAsync<Pitch>(Query.PITCH__GET_BY_ID, new Dictionary<string, object>
                {
                    {"@id", id }
                });
                return pitch;
            }
        }

        public async Task<object> GetCountPaging(int pageIndex, int pageSize, List<WhereCondition> where)
        {
            var whereQuery = BuildWhereQuery(where);
            var countQuery = $@"select  count(1) 
                                        from pitchs 
                                        where {whereQuery}";

            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();

                var dic = new Dictionary<string, object>();
                for (int i = 0; i < where.Count; i++)
                {
                    var item = string.Format("@where{0}", i);
                    dic.Add(item, where[i].Value);
                }

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

        public async Task<List<Pitch>> GetPaging(int pageIndex, int pageSize, List<WhereCondition> where)
        {
            var whereQuery = BuildWhereQuery(where);
            var query = $@"select   Id, 
                                    Name, 
                                    Status, 
                                    NameDetails, 
                                    Address, 
                                    ManagerId 
                                    from pitchs 
                                    where {whereQuery} 
                                    order by Name 
                                    limit @offset, @pageSize";
            var countQuery = $@"select  count(1) 
                                        from pitchs 
                                        where {whereQuery}";
            
            using(var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();

                for (int i = 0; i < where.Count; i++)
                {
                    var item = string.Format("@where{0}", i);
                    dic.Add(item, where[i].Value);
                }

                var totalRecord = await connection.QuerySingleAsync<int>(countQuery, dic);
                var totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                if (pageIndex > totalPage)
                {
                    pageIndex = 1;
                }
                var offSet = (pageIndex - 1) * pageSize;

                dic.Add("@offset", offSet);
                dic.Add("@pageSize", pageSize);
                var lstPitch = await connection.QueryAsync<Pitch>(query, dic);
                return lstPitch.ToList();
            }
        }

        public async Task<bool> Insert(Pitch pitch)
        {
            var query = @"insert into pitchs    (   Id, 
                                                    Name, 
                                                    Address, 
                                                    MinutesPerMatch, 
                                                    Quantity, 
                                                    TimeSlotPerDay, 
                                                    ManagerId, 
                                                    Status, 
                                                    NameDetails, 
                                                    CreatedDate, 
                                                    CreatedBy, 
                                                    ModifiedDate, 
                                                    ModifiedBy)
                            value ( @id, 
                                    @name, 
                                    @address, 
                                    @minutesPerMatch, 
                                    @quantity, 
                                    @timeSlotPerDay, 
                                    @managerId, 
                                    @status, 
                                    @nameDetails, 
                                    @createdDate, 
                                    @createdBy, 
                                    @modifiedDate, 
                                    @modifiedBy)";

            var queryChild = @"insert into time_frame_infos (   Id, 
                                                                Name, 
                                                                SortOrder, 
                                                                TimeBegin, 
                                                                TimeEnd, 
                                                                Price, 
                                                                PitchId, 
                                                                CreatedDate, 
                                                                CreatedBy, 
                                                                ModifiedDate, 
                                                                ModifiedBy)
                                value ( @id, 
                                        @name, 
                                        @sortOrder, 
                                        @timeBegin, 
                                        @timeEnd, 
                                        @price, 
                                        @pitchId, 
                                        @createdDate, 
                                        @createdBy, 
                                        @modifiedDate, 
                                        @modifedBy)";
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                var dic = new Dictionary<string, object?>();
                dic.Add("@id", pitch.Id);
                dic.Add("@name", pitch.Name);
                dic.Add("@address", pitch.Address);
                dic.Add("@minutesPerMatch", pitch.MinutesPerMatch);
                dic.Add("@quantity", pitch.Quantity);
                dic.Add("@timeSlotPerDay", pitch.TimeSlotPerDay);
                dic.Add("@managerId", pitch.ManagerId);
                dic.Add("@status", pitch.Status);
                dic.Add("@nameDetails", pitch.NameDetails);
                dic.Add("@createdDate", pitch.CreatedDate);
                dic.Add("@createdBy", pitch.CreatedBy);
                dic.Add("@modifiedDate", pitch.ModifiedDate);
                dic.Add("@modifiedBy", pitch.ModifiedBy);
                var affect = await connection.ExecuteAsync(query, dic, transaction);
                if(affect > 0)
                {
                    for (int i = 0; i < pitch.TimeFrameInfos.Count; i++)
                    {
                        var item = pitch.TimeFrameInfos[i];
                        dic = new Dictionary<string, object?>();
                        dic.Add("@id", item.Id);
                        dic.Add("@name", item.Name);
                        dic.Add("@sortOrder", item.SortOrder);
                        dic.Add("@timeBegin", item.TimeBegin);
                        dic.Add("@timeEnd", item.TimeEnd);
                        dic.Add("@price", item.Price);
                        dic.Add("@pitchId", item.PitchId);
                        dic.Add("@createdDate", item.CreatedDate);
                        dic.Add("@createdBy", item.CreatedBy);
                        dic.Add("@modifiedDate", item.ModifiedDate);
                        dic.Add("@modifiedBy", item.ModifiedBy);

                        affect = await connection.ExecuteAsync(queryChild, dic, transaction);
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

        public async Task<bool> Update(Pitch pitch)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                var dic = new Dictionary<string, object?>();
                dic.Add("@id", pitch.Id);
                dic.Add("@name", pitch.Name);
                dic.Add("@address", pitch.Address);
                dic.Add("@minutesPerMatch", pitch.MinutesPerMatch);
                dic.Add("@quantity", pitch.Quantity);
                dic.Add("@timeSlotPerDay", pitch.TimeSlotPerDay);
                dic.Add("@status", pitch.Status);
                dic.Add("@nameDetails", pitch.NameDetails);
                dic.Add("@modifiedDate", pitch.ModifiedDate);
                dic.Add("@modifiedBy", pitch.ModifiedBy);
                var affect = await connection.ExecuteAsync(@"update pitchs set 
                                                                                Name = @name, 
                                                                                Address = @address, 
                                                                                MinutesPerMatch = @minutesPerMatch, 
                                                                                Quantity = @quantity, 
                                                                                TimeSlotPerDay = @timeSlotPerDay,
                                                                                Status = @status, 
                                                                                NameDetails = @nameDetails,
                                                                                ModifiedBy = @modifiedBy,
                                                                                ModifiedDate = @modifiedDate
                                                                                where Id = @id", dic, transaction);
                if(affect > 0)
                {
                    dic = new Dictionary<string, object?>();
                    dic.Add("@pitchId", pitch.Id);
                    affect = await connection.ExecuteAsync("delete from time_frame_infos where PitchId = @pitchId", dic, transaction);
                    for (int i = 0; i < pitch.TimeFrameInfos.Count; i++)
                    {
                        var item = pitch.TimeFrameInfos[i];
                        dic = new Dictionary<string, object?>();
                        dic.Add("@id", item.Id);
                        dic.Add("@name", item.Name);
                        dic.Add("@sortOrder", item.SortOrder);
                        dic.Add("@timeBegin", item.TimeBegin);
                        dic.Add("@timeEnd", item.TimeEnd);
                        dic.Add("@price", item.Price);
                        dic.Add("@pitchId", item.PitchId);
                        dic.Add("@createdDate", item.CreatedDate);
                        dic.Add("@createdBy", item.CreatedBy);
                        dic.Add("@modifiedDate", item.ModifiedDate);
                        dic.Add("@modifiedBy", item.ModifiedBy);

                        affect = await connection.ExecuteAsync(@"insert into time_frame_infos (Id, Name, SortOrder, TimeBegin, TimeEnd, Price, PitchId, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                                value (@id, @name, @sortOrder, @timeBegin, @timeEnd, @price, @pitchId, @createdDate, @createdBy, @modifiedDate, @modifedBy)", dic, transaction);
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
    }
}
