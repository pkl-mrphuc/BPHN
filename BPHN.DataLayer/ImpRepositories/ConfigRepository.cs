using BPHN.DataLayer.IRepositories;
using BPHN.ModelLayer;
using Dapper;
using Microsoft.Extensions.Options;

namespace BPHN.DataLayer.ImpRepositories
{
    public class ConfigRepository : BaseRepository, IConfigRepository
    {
        public ConfigRepository(IOptions<AppSettings> appSettings) : base(appSettings)
        {

        }

        public async Task<List<Config>> GetConfigs(Guid accountId, string? key = null)
        {
            return !string.IsNullOrWhiteSpace(key) ? await GetConfigsByKey(accountId, key) : await GetConfigs(accountId);
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

        private async Task<List<Config>> GetConfigs(Guid accountId)
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

        private async Task<List<Config>> GetConfigsByKey(Guid accountId, string key)
        {
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                var configs = (await connection.QueryAsync<Config>(Query.CONFIG__GET_BY_KEY, new Dictionary<string, object>
                {
                    { "@accountId", accountId },
                    { "@key", key }
                })).ToList();
                return configs;
            }
        }
    }
}
