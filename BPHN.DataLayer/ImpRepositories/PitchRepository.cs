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

        public object GetCountPaging(int pageIndex, int pageSize, string txtSearch)
        {
            return new { TotalPage = 1, TotalRecordCurrentPage = 10, TotalAllRecords = 100 };
        }

        public List<Pitch> GetPaging(int pageIndex, int pageSize, string txtSearch)
        {
            return new List<Pitch>();
        }

        public bool Insert(Pitch pitch)
        {
            string query = @"insert into pitchs(Id, Name, Address, MinutesPerMatch, Quantity, TimeSlotPerDay, ManagerId, Status, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                            value (@id, @name, @address, @minutesPerMatch, @quantity, @timeSlotPerDay, @managerId, @status, @createdDate, @createdBy, @modifiedDate, @modifiedBy)";

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
    }
}
