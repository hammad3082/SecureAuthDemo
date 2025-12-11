using SecureAuthDemo.Enums;
using SecureAuthDemo.Services.Cache;
using SecureAuthDemo.Services.Interfaces;
using System.Text.Json;

namespace SecureAuthDemo.Services.Implimentations
{
    public class StateStore : IStateStore
    {
        private readonly ICacheService _cache;

        public StateStore(ICacheService cache)
        {
            _cache = cache;
        }

        public async Task SetStateAsync(string state, AuthProvider provider, TimeSpan ttl)
        {
            string json = JsonSerializer.Serialize(provider);
            await _cache.SetAsync(GetKey(state), json, ttl);
        }

        public async Task RemoveStateAsync(string state)
        {
            await _cache.RemoveAsync(GetKey(state));
        }

        public async Task<(bool Found, AuthProvider Provider)> TryGetProviderAsync(string state)
        {
            var json = await _cache.GetAsync(GetKey(state));
            if (json == null)
                return (false, default);

            var provider = JsonSerializer.Deserialize<AuthProvider>(json)!;
            return (true, provider);
        }

        private string GetKey(string state) => $"oauth:state:{state}";
    }
}
