using Microsoft.Extensions.Caching.Memory;

namespace CrossCutting
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> func, TimeSpan duration);
    }

    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetOrCreateAsync<T>(string keyCache, Func<Task<T>> func, TimeSpan duration)
        {
            var result = await _cache.GetOrCreateAsync(keyCache, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = duration;
                return await func();
            });

            return result;
        }
    }
}
