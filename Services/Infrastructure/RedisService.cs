using StackExchange.Redis;

namespace SecureAuthDemo.Services.Cache
{
    public class RedisService : ICacheService
    {
        private readonly IDatabase? _db;
        public RedisService(IConnectionMultiplexer redis)
        {
            _db = redis?.GetDatabase();
        }

        public bool IsAvailable => _db != null;

        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            await _db.StringSetAsync(key, value, expiry);
        }
        public async Task<string?> GetAsync(string key)
        {
            return await _db.StringGetAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }
    }
}
