using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface IConfigService
    {
        Task<ServiceResultModel> GetConfigs(string? key = null);
        Task<ServiceResultModel> Save(List<Config> configs);
    }
}
