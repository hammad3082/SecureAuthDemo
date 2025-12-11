using Microsoft.Extensions.Caching.Memory;

namespace SecureAuthDemo.Services.Cache
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            _cache.Set(key, value, expiry ?? TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        public Task<string?> GetAsync(string key)
        {
            return Task.FromResult(_cache.TryGetValue(key, out string? value) ? value : null);
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
