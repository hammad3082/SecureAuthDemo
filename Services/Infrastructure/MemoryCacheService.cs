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

            if (_cache.TryGetValue(key, out object? value) && value != null)
            {
                return Task.FromResult<string?>(value.ToString());
            }

            return Task.FromResult<string?>(null);
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task<long> IncrementAsync(string key)
        {
            _cache.TryGetValue(key, out long currentValue);
            long newValue = currentValue + 1;
            _cache.Set(key, newValue);
            return Task.FromResult(newValue);
        }

        public Task<long> DecrementAsync(string key)
        {
            _cache.TryGetValue(key, out long currentValue);
            long newValue = currentValue - 1;
            if (newValue < 0) newValue = 0;
            _cache.Set(key, newValue);
            return Task.FromResult(newValue);
        }
    }
}
