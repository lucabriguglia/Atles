using System;
using System.Threading.Tasks;

namespace Atles.Data.Caching
{
    public interface ICacheManager
    {
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> acquireAsync);

        Task<T> GetOrSetAsync<T>(string key, int cacheTime, Func<Task<T>> acquireAsync);

        T GetOrSet<T>(string key, Func<T> acquire);

        T GetOrSet<T>(string key, int cacheTime, Func<T> acquire);

        void Remove(string key);
    }
}
