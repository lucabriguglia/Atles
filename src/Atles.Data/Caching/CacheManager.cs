using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Atles.Data.Caching
{
    public class CacheManager : ICacheManager
    {
        private readonly IMemoryCache _memoryCache;

        public CacheManager(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> acquireAsync)
        {
            return GetOrSetAsync(key, 60, acquireAsync);
        }

        public async Task<T> GetOrSetAsync<T>(string key, int cacheTime, Func<Task<T>> acquireAsync)
        {
            if (_memoryCache.TryGetValue(key, out T data))
            {
                return data;
            }

            data = await acquireAsync();

            var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheTime));

            _memoryCache.Set(key, data, memoryCacheEntryOptions);

            return data;
        }

        public T GetOrSet<T>(string key, Func<T> acquire)
        {
            return GetOrSet(key, 60, acquire);
        }

        public T GetOrSet<T>(string key, int cacheTime, Func<T> acquire)
        {
            if (_memoryCache.TryGetValue(key, out T data))
            {
                return data;
            }

            data = acquire();

            var memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(cacheTime));

            _memoryCache.Set(key, data, memoryCacheEntryOptions);

            return data;
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }
}
