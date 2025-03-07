using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Dapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BPHN.DataLayer.ImpRepositories
{
    public class HistoryLogRepository : BaseRepository, IHistoryLogRepository
    {
        public HistoryLogRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {

        }

        public async Task<object> GetCountPaging(int pageIndex, int pageSize, List<WhereCondition> where)
        {
            var whereQuery = BuildWhereQuery(where);

            var dic = new Dictionary<string, object>();
            for (int i = 0; i < where.Count; i++)
            {
                var item = string.Format("@where{0}", i);
                dic.Add(item, where[i].Value);
            }
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var totalRecord = await connection.QuerySingleAsync<int>($"select count(1) from history_logs where {whereQuery}", dic);
                var totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                var totalRecordCurrentPage = 0;
                if(totalRecord > 0)
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

        public async Task<HistoryLogDescription?> GetDescription(string historyLogId)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var data = await connection.QueryFirstAsync<HistoryLogDescription>(Query.HISTORY_LOG__GET_DESCRIPTION, new Dictionary<string, object>
                {
                    { "@id", historyLogId }
                });
                return data;
            }
        }

        public async Task<List<HistoryLog>> GetPaging(int pageIndex, int pageSize, List<WhereCondition> where)
        {
            var whereQuery = BuildWhereQuery(where);

            var dic = new Dictionary<string, object>();
            for (int i = 0; i < where.Count; i++)
            {
                var item = string.Format("@where{0}", i);
                dic.Add(item, where[i].Value);
            }
            var query = $"select Id, CreatedDate, IPAddress, Actor, ActionName, Entity, Description from history_logs where {whereQuery} order by CreatedDate desc limit @offSet, @pageSize";


            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var totalRecord = await connection.QuerySingleAsync<int>($"select count(1) from history_logs where {whereQuery}", dic);
                var totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                if(pageIndex > totalPage)
                {
                    pageIndex = 1;
                }
                var offSet = (pageIndex - 1) * pageSize;

                dic.Add("@offSet", offSet);
                dic.Add("@pageSize", pageSize);
                var lstHistoryLog = await connection.QueryAsync<HistoryLog>(query, dic);
                return lstHistoryLog.ToList();
            }
        }

        public async Task<bool> Write(HistoryLog history)
        {
            var query = @"insert into history_logs(Id, IPAddress, ActionName, Actor, ActorId, Entity, Description, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                                value (@id, @ipAddress, @actionName, @actor, @actorId, @entity, @description, @createdDate, @createdBy, @modifiedDate, @modifiedBy);";
            
            var dic = new Dictionary<string, object?>();
            dic.Add("@id", history.Id);
            dic.Add("@ipAddress", history.IPAddress);
            dic.Add("@actionName", history.ActionName);
            dic.Add("@actor", history.Actor);
            dic.Add("@actorId", history.ActorId);
            dic.Add("@entity", history.Entity);
            dic.Add("@description", history.Description);
            if (history.Data != null)
            {
                query += "insert into history_log_descriptions(Id, ModelId, OldData, NewData) value (@id, @modelId, @oldData, @newData);";
                dic.Add("@modelId", history.Data.ModelId);
                dic.Add("@oldData", history.Data.OldData);
                dic.Add("@newData", history.Data.NewData);
            }
            dic.Add("@createdDate", history.CreatedDate);
            dic.Add("@createdBy", history.CreatedBy);
            dic.Add("@modifiedDate", history.ModifiedDate);
            dic.Add("@modifiedBy", history.ModifiedBy);
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var affect = await connection.ExecuteAsync(query, dic);
                if(affect == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
