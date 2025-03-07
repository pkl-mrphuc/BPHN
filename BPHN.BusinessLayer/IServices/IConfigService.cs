using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IConfigService
    {
        Task<ServiceResultModel> GetConfigs(string? key = null);
        Task<Config> GetByKey(Guid accountId, string key);
        Task<string> GetValueByKey(Guid accountId, string key);
        Task<List<Config>> GetConfigs(Guid accountId);
        Task<Dictionary<string, string>> GetByKey(Guid accountId, params string[] keys);
        Task<ServiceResultModel> Save(List<Config> configs);
        Task<bool> AllowMultiUser(Guid accountId);
        Task<string> Language(Guid accountId);
    }
}
