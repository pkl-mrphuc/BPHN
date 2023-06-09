using BPHN.ModelLayer;

namespace BPHN.DataLayer.IRepositories
{
    public interface IConfigRepository
    {
        Task<List<Config>> GetConfigs(Guid accountId, string key = null);
        Task<bool> Save(List<Config> configs);
    }
}
