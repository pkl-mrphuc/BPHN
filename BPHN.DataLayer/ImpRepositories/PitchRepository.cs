using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Dapper;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.DataLayer.ImpRepositories
{
    public class PitchRepository : BaseRepository, IPitchRepository
    {
        public PitchRepository(IOptions<AppSettings> appSetting) : base(appSetting)
        {
                
        }

        public Pitch? GetById(string id)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();
                dic.Add("@id", id);
                var pitch = connection.QueryFirstOrDefault<Pitch>("select * from pitchs where id = @id", dic);
                if(pitch != null)
                {
                    dic = new Dictionary<string, object>();
                    dic.Add("@pitchId", pitch.Id);
                    var lstTimeFrame = connection.Query<TimeFrameInfo>("select * from time_frame_infos where PitchId = @pitchId", dic);
                    if(lstTimeFrame != null && lstTimeFrame.Count() > 0)
                    {
                        pitch.TimeFrameInfos = lstTimeFrame.ToList();
                    }
                }
                return pitch;
            }
        }

        public object GetCountPaging(int pageIndex, int pageSize, List<WhereCondition> where)
        {
            string whereQuery = BuildWhereQuery(where);
            string countQuery = $@"select count(*) from pitchs where {whereQuery}";

            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();

                var dic = new Dictionary<string, object>();
                for (int i = 0; i < where.Count; i++)
                {
                    var item = string.Format("@where{0}", i);
                    dic.Add(item, where[i].Value);
                }

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

        public List<Pitch> GetPaging(int pageIndex, int pageSize, List<WhereCondition> where)
        {
            string whereQuery = BuildWhereQuery(where);
            string query = $@"select * from pitchs where {whereQuery} order by Name limit @offset, @pageSize";
            string countQuery = $@"select count(*) from pitchs where {whereQuery}";
            
            using(var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var dic = new Dictionary<string, object>();

                for (int i = 0; i < where.Count; i++)
                {
                    var item = string.Format("@where{0}", i);
                    dic.Add(item, where[i].Value);
                }

                int totalRecord = connection.QuerySingle<int>(countQuery, dic);
                int totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                if (pageIndex > totalPage)
                {
                    pageIndex = 1;
                }
                int offSet = (pageIndex - 1) * pageSize;

                dic.Add("@offset", offSet);
                dic.Add("@pageSize", pageSize);
                return connection.Query<Pitch>(query, dic).ToList();
            }
        }

        public bool Insert(Pitch pitch)
        {
            string query = @"insert into pitchs(Id, Name, Address, MinutesPerMatch, Quantity, TimeSlotPerDay, ManagerId, Status, NameDetails, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                            value (@id, @name, @address, @minutesPerMatch, @quantity, @timeSlotPerDay, @managerId, @status, @nameDetails, @createdDate, @createdBy, @modifiedDate, @modifiedBy)";

            string queryChild = @"insert into time_frame_infos (Id, Name, SortOrder, TimeBegin, TimeEnd, Price, PitchId, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                                value (@id, @name, @sortOrder, @timeBegin, @timeEnd, @price, @pitchId, @createdDate, @createdBy, @modifiedDate, @modifedBy)";
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();

                Dictionary<string, object> dic = new Dictionary<string, object>();
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
                int affect = connection.Execute(query, dic, transaction);
                if(affect > 0)
                {
                    for (int i = 0; i < pitch.TimeFrameInfos.Count; i++)
                    {
                        var item = pitch.TimeFrameInfos[i];
                        dic = new Dictionary<string, object>();
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

                        affect = connection.Execute(queryChild, dic, transaction);
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

        public bool Update(Pitch pitch)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var transaction = connection.BeginTransaction();
                var dic = new Dictionary<string, object>();
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
                var affect = connection.Execute(@"update pitchs set 
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
                    dic = new Dictionary<string, object>();
                    dic.Add("@pitchId", pitch.Id);
                    affect = connection.Execute("delete from time_frame_infos where PitchId = @pitchId", dic, transaction);
                    for (int i = 0; i < pitch.TimeFrameInfos.Count; i++)
                    {
                        var item = pitch.TimeFrameInfos[i];
                        dic = new Dictionary<string, object>();
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

                        affect = connection.Execute(@"insert into time_frame_infos (Id, Name, SortOrder, TimeBegin, TimeEnd, Price, PitchId, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
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
