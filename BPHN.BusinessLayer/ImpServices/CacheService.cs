using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPHN.BusinessLayer.ImpServices
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly AppSettings _appSettings;
        public CacheService(IDistributedCache cache, IOptions<AppSettings> appSettings)
        {
            _cache = cache;
            _appSettings = appSettings.Value;
        }

        public string Get(string key)
        {
            var value = _cache.GetStringAsync(key).Result;
            if (value != null) return value;
            return string.Empty;
        }

        public async Task<string> GetAsync(string key)
        {
            var value = await _cache.GetStringAsync(key);
            if (value != null) return value;
            return string.Empty;
        }

        public string GetKeyCache(string key, string model)
        {
            return string.Format("{0}_{1}_Cache", key, model);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public void Set(string key, string value, int? expireHour = null)
        {
            if (_appSettings.RedisExpireHour == 0)
            {
                expireHour = Constansts.EXPIRE_HOUR_REDIS_CACHE;
            }

            if (expireHour == null)
            {
                expireHour = _appSettings.RedisExpireHour;
            }

            double expireSecond = Convert.ToDouble(expireHour * 60 * 60);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSecond),
            };
            _cache.SetString(key, value, options);
        }

        public async Task SetAsync(string key, string value, int? expireHour = null)
        {
            if(_appSettings.RedisExpireHour == 0)
            {
                expireHour = Constansts.EXPIRE_HOUR_REDIS_CACHE;
            }

            if (expireHour == null)
            {
                expireHour = _appSettings.RedisExpireHour;
            }

            double expireSecond = Convert.ToDouble(expireHour * 60 * 60);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSecond),
            };
            await _cache.SetStringAsync(key, value, options);
        }
    }
}
