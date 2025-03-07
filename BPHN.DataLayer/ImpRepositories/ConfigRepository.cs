using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Dapper;
using Microsoft.Extensions.Options;

namespace BPHN.DataLayer.ImpRepositories
{
    public class ConfigRepository : BaseRepository, IConfigRepository
    {
        public ConfigRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {

        }

        public async Task<bool> Save(List<Config> configs)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                using (var tranction = connection.BeginTransaction())
                {
                    foreach (var config in configs)
                    {
                        var dic = new Dictionary<string, object?>();
                        dic.Add("@id", config.Id);
                        dic.Add("@accountId", config.AccountId);
                        dic.Add("@key", config.Key);
                        dic.Add("@value", config.Value);
                        dic.Add("@createdDate", config.CreatedDate);
                        dic.Add("@createdBy", config.CreatedBy);
                        dic.Add("@modifiedDate", config.ModifiedDate);
                        dic.Add("@modifiedBy", config.ModifiedBy);
                        var query = @"set @idConfig = (select c.id from configs c where c.AccountId = @accountId and c.`Key` = @key);
                                        set @idConfig = if(@idConfig is null, @id, @idConfig);
                                        insert into configs(Id, AccountId, `Key`, Value, CreatedDate, CreatedBy, ModifiedDate, ModifiedBy)
                                            value(@idConfig, @accountId, @key, @value, @createdDate, @createdBy, @modifiedDate, @modifiedBy)
                                        on duplicate key update Value = @value, ModifiedDate = @modifiedDate, ModifiedBy = @modifiedBy";
                        var affect = await connection.ExecuteAsync(query, dic, tranction, commandType: System.Data.CommandType.Text);
                        if (affect == 0)
                        {
                            tranction.Rollback();
                            return false;
                        }
                    }
                    tranction.Commit();
                }
            }
            return true;
        }

        public async Task<List<Config>> GetConfigs(Guid accountId)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var configs = (await connection.QueryAsync<Config>(Query.CONFIG__GET_ALL, new Dictionary<string, object>
                {
                    { "@accountId", accountId }
                })).ToList();
                return configs;
            }
        }

        public async Task<Dictionary<string, string>> GetByKey(Guid accountId, params string[] keys)
        {
            var where = BuildWhere(new List<WhereCondition>
            {
                new WhereCondition
                {
                    Column = "c.AccountId",
                    Operator = "=",
                    Value = accountId.ToString()
                },
                new WhereCondition
                {
                    Column = "c.Key",
                    Operator = "in",
                    Values = keys
                }
            });
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var configs = (await connection.QueryAsync<Config>($"select c.Key, c.Value from configs c where {where.filter}", where.param)).ToDictionary(x => x.Key, x => x.Value);
                return keys.ToDictionary(x => x, x => configs.TryGetValue(x, out var value) ? value : string.Empty);
            }
        }

        public async Task<Config> GetByKey(Guid accountId, string key)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var config = (await connection.QueryFirstOrDefaultAsync<Config>(Query.CONFIG__GET_ALL, new Dictionary<string, object>
                {
                    { "@accountId", accountId },
                    { "@key", key }
                }));
                return config;
            }
        }

        public async Task<string> GetValueByKey(Guid accountId, string key)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var config = (await connection.QueryFirstOrDefaultAsync<string>(Query.CONFIG__GET_ALL, new Dictionary<string, object>
                {
                    { "@accountId", accountId },
                    { "@key", key }
                }));
                return config;
            }
        }
    }
}
