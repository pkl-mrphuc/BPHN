using BPHN.ModelLayer;

namespace BPHN.BusinessLayer.IServices
{
    public interface ICacheService
    {
        string Get(string key);
        Task<string> GetAsync(string key);
        void Set(string key, string value, int? expireHour = null);
        Task SetAsync(string key, string value, int? expireHour = null);
        void Remove(string key);
        Task RemoveAsync(string key);
        string GetKeyCache(Guid id, EntityEnum model, string key = "");
        string GetKeyCache(string id, EntityEnum model, string key = "");
    }
}
