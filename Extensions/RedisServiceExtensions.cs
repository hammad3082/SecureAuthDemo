using SecureAuthDemo.Services;
using StackExchange.Redis;

namespace SecureAuthDemo.Extensions
{
    public static class RedisServiceExtensions
    {
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
