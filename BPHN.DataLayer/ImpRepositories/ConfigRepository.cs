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
            using (var connection = ConnectDB(GetConnectionString()))
            {
                connection.Open();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@accountId", accountId);
                var query = $"select c.Key, c.Value from configs c where c.AccountId = @accountId";
                if (!string.IsNullOrEmpty(key))
                {
                    dic.Add("@key", key);
                    query = $"select c.Key, c.Value from configs c where c.AccountId = @accountId and c.Key in (@key)";
                }
                var configs = (await connection.QueryAsync<Config>(query, dic)).ToList();
                return configs;
            }
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
                        if(affect == 0)
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
    }
}
