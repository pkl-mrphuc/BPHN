using BPHN.BusinessLayer.IServices;
using BPHN.ModelLayer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

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
            var result = string.Empty;
            if(_appSettings.EnableCacheService)
            {
                try
                {
                    var value = _cache.GetStringAsync(key).Result;
                    if (value is not null)
                    {
                        result = value;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Cache/Get error: {JsonConvert.SerializeObject(ex)}");
                }
            }
            return result;
        }

        public async Task<string> GetAsync(string key)
        {
            var result = string.Empty;
            if(_appSettings.EnableCacheService)
            {
                try
                {
                    var value = await _cache.GetStringAsync(key);
                    if (value is not null)
                    {
                        result = value;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Cache/GetAsync error: {JsonConvert.SerializeObject(ex)}");
                }
            }
            return result;
        }

        public string GetKeyCache(Guid id, EntityEnum model, string key = "")
        {
            return $"{key}{id}_{model}";
        }

        public string GetKeyCache(string id, EntityEnum model, string key = "")
        {
            return $"{key}{id}_{model}";
        }

        public void Remove(string key)
        {
            if(_appSettings.EnableCacheService)
            {
                try
                {
                    _cache.Remove(key);
                }
                catch (Exception ex)
                {
                    Log.Error($"Cache/Remove error: {JsonConvert.SerializeObject(ex)}");
                }
            }
        }

        public async Task RemoveAsync(string key)
        {
            if (_appSettings.EnableCacheService)
            {
                try
                {
                    await _cache.RemoveAsync(key);
                }
                catch (Exception ex)
                {
                    Log.Error($"Cache/RemoveAsync error: {JsonConvert.SerializeObject(ex)}");
                }
            }
        }

        public void Set(string key, string value, int? expireHour = null)
        {
            if (_appSettings.EnableCacheService)
            {
                if (_appSettings.RedisExpireHour == 0)
                {
                    expireHour = Constansts.EXPIRE_HOUR_REDIS_CACHE;
                }

                if (expireHour is null)
                {
                    expireHour = _appSettings.RedisExpireHour;
                }

                var expireSecond = Convert.ToDouble(expireHour * 60 * 60);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSecond),
                };

                try
                {
                    _cache.SetString(key, value, options);
                }
                catch (Exception ex)
                {
                    Log.Error($"Cache/Set error: {JsonConvert.SerializeObject(ex)}");
                }
            }
        }

        public async Task SetAsync(string key, string value, int? expireHour = null)
        {
            if (_appSettings.EnableCacheService)
            {
                if (_appSettings.RedisExpireHour == 0)
                {
                    expireHour = Constansts.EXPIRE_HOUR_REDIS_CACHE;
                }

                if (expireHour is null)
                {
                    expireHour = _appSettings.RedisExpireHour;
                }

                var expireSecond = Convert.ToDouble(expireHour * 60 * 60);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expireSecond),
                };

                try
                {
                    await _cache.SetStringAsync(key, value, options);
                }
                catch (Exception ex)
                {
                    Log.Error($"Cache/SetAsync error: {JsonConvert.SerializeObject(ex)}");
                }
            }
        }
    }
}
