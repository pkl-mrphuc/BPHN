using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        string GetKeyCache(string key,string model);
    }
}
