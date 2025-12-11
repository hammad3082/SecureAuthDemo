using SecureAuthDemo.Services.Cache;

namespace SecureAuthDemo.Extensions
{
    public static class CacheExtensions
    {
        public static IServiceCollection AddCacheServices(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddMemoryCache();
            services.AddSingleton<MemoryCacheService>();

            services.AddRedisService(config);

            var useRedis = config.GetValue<bool>("CacheSettings:UseRedis");

            // Choose Redis if available, otherwise MemoryCache
            services.AddSingleton<ICacheService>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<ICacheService>>();

                if (useRedis)
                {
                    var redis = sp.GetService<RedisService>(); // safe
                    if (redis != null)
                    {
                        logger.LogInformation("Using Redis Cache");
                        return redis;
                    }

                    logger.LogWarning("Redis not available. Falling back to MemoryCache.");
                }

                return sp.GetRequiredService<MemoryCacheService>();
            });

            return services;
        }
    }
}
