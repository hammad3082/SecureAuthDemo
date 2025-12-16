using SecureAuthDemo.Enums;

namespace SecureAuthDemo.Services.Auth.Abstractions
{
    public interface IStateStore
    {
        Task SetStateAsync(string state, AuthProvider provider, TimeSpan ttl);
        Task<(bool Found, AuthProvider Provider)> TryGetProviderAsync(string state);
        Task RemoveStateAsync(string state);
    }
}
