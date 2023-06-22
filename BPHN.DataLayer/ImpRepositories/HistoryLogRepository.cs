using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Dapper;
using Microsoft.Extensions.Options;

namespace BPHN.DataLayer.ImpRepositories
{
    public class HistoryLogRepository : BaseRepository, IHistoryLogRepository
    {
        public HistoryLogRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {

        }

        public async Task<object> GetCountPaging(int pageIndex, int pageSize, List<WhereCondition> where)
        {
            string whereQuery = BuildWhereQuery(where);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            for (int i = 0; i < where.Count; i++)
            {
                var item = string.Format("@where{0}", i);
                dic.Add(item, where[i].Value);
            }
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                int totalRecord = await connection.QuerySingleAsync<int>($"select count(*) from history_logs where {whereQuery}", dic);
                int totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                int totalRecordCurrentPage = 0;
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

        public async Task<List<HistoryLog>> GetPaging(int pageIndex, int pageSize, List<WhereCondition> where)
        {
            string whereQuery = BuildWhereQuery(where);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            for (int i = 0; i < where.Count; i++)
            {
                var item = string.Format("@where{0}", i);
                dic.Add(item, where[i].Value);
            }
            string query = $"select * from history_logs where {whereQuery} order by CreatedDate desc limit @offSet, @pageSize";


            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                int totalRecord = await connection.QuerySingleAsync<int>($"select count(*) from history_logs where {whereQuery}", dic);
                int totalPage = totalRecord % pageSize == 0 ? totalRecord / pageSize : (totalRecord / pageSize) + 1;
                if(pageIndex > totalPage)
                {
                    pageIndex = 1;
                }
                int offSet = (pageIndex - 1) * pageSize;

                dic.Add("@offSet", offSet);
                dic.Add("@pageSize", pageSize);
                var lstHistoryLog = await connection.QueryAsync<HistoryLog>(query, dic);
                return lstHistoryLog.ToList();
            }
        }

        public async Task<bool> Write(HistoryLog history)
        {
            string query = @"insert into history_logs(Id, ActionName, Actor, ActorId, Entity, Description, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                                value (@id, @actionName, @actor, @actorId, @entity, @description, @createdDate, @createdBy, @modifiedDate, @modifiedBy)";
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("@id", history.Id);
            dic.Add("@actionName", history.ActionName);
            dic.Add("@actor", history.Actor);
            dic.Add("@actorId", history.ActorId);
            dic.Add("@entity", history.Entity);
            dic.Add("@description", history.Description);
            dic.Add("@createdDate", history.CreatedDate);
            dic.Add("@createdBy", history.CreatedBy);
            dic.Add("@modifiedDate", history.ModifiedDate);
            dic.Add("@modifiedBy", history.ModifiedBy);
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                int affect = await connection.ExecuteAsync(query, dic);
                if(affect == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
