using SecureAuthDemo.Services.Cache;
using StackExchange.Redis;

namespace SecureAuthDemo.Extensions
{
    public static class RedisServiceExtensions
    {
        // This method *tries* to connect and only registers RedisService when successful.
        public static IServiceCollection AddRedisService(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<RedisService>>();

                try
                {
                    var host = config["RedisSettings:Host"];
                    var port = config["RedisSettings:Port"];
                    var password = config["RedisSettings:Password"];

                    if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(port))
                    {
                        logger.LogWarning("Redis settings incomplete. Skipping Redis registration.");
                        return null!;
                    }

                    var redisConfig = ConfigurationOptions.Parse($"{host}:{port}");

                    if (!string.IsNullOrEmpty(password))
                        redisConfig.Password = password;

                    redisConfig.Ssl = false;

                    var multiplexer = ConnectionMultiplexer.Connect(redisConfig);
                    logger.LogInformation("Redis connected successfully.");
                    return multiplexer;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to initialize Redis.");
                    return null!;
                }
            });

            services.AddSingleton<RedisService>();

            return services;
        }

        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var host = configuration["RedisSettings:Host"];
            var port = configuration["RedisSettings:Port"];
            var password = configuration["RedisSettings:Password"];

            var redisConfig = ConfigurationOptions.Parse($"{host}:{port}");
            redisConfig.Password = password;
            redisConfig.Ssl = false;

            var multiplexer = ConnectionMultiplexer.Connect(redisConfig);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            services.AddScoped<ICacheService, RedisService>();

            return services;
        }
    }
}
