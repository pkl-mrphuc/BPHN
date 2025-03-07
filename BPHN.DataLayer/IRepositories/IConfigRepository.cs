using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface IConfigRepository
    {
        Task<List<Config>> GetConfigs(Guid accountId);
        Task<Config> GetByKey(Guid accountId, string key);
        Task<string> GetValueByKey(Guid accountId, string key);
        Task<Dictionary<string, string>> GetByKey(Guid accountId, params string[] keys);
        Task<bool> Save(List<Config> configs);
    }
}
