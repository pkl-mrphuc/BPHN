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
                if (pageIndex > totalPage)
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
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    var affect = await connection.ExecuteAsync(Query.HISTORY_LOG__INSERT, new Dictionary<string, object?>
                    {
                        { "@id", history.Id },
                        { "@ipAddress", history.IPAddress },
                        { "@actionName", history.ActionName },
                        { "@actor", history.Actor },
                        { "@actorId", history.ActorId },
                        { "@entity", history.Entity },
                        { "@description", history.Description },
                        { "@createdDate", history.CreatedDate },
                        { "@createdBy", history.CreatedBy },
                        { "@modifiedDate", history.ModifiedDate },
                        { "@modifiedBy", history.ModifiedBy },
                    }, transaction);

                    if (history.Data is not null)
                    {
                        affect = await connection.ExecuteAsync(Query.HISTORY_LOG__INSERT_DESCRIPTION, new Dictionary<string, object?>
                        {
                            { "@id", history.Id },
                            { "@modelId", history.Data.ModelId },
                            { "@oldData", history.Data.OldData },
                            { "@newData", history.Data.NewData },
                        }, transaction);
                    }

                    if (affect <= 0)
                    {
                        transaction.Rollback();
                    }
                    else
                    {
                        transaction.Commit();
                    }
                    return affect > 0;
                }
            }
        }
    }
}
