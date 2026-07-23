namespace SecureAuthDemo.Services.Cache
{
    public interface ICacheService
    {
        Task SetAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetAsync(string key);
        Task RemoveAsync(string key);

        // For atomic counting
        Task<long> IncrementAsync(string key);
        Task<long> DecrementAsync(string key);
    }
}
