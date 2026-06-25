using Microsoft.AspNetCore.Cors.Infrastructure;
using SecureAuthDemo.Services.Cache;
using SecureAuthDemo.Constants;

namespace SecureAuthDemo.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCustomCors(
            this IServiceCollection services,
            IConfiguration config)
        {
            var allowedOrigins = config.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

            services.AddCors(options =>
            {
                options.AddPolicy(name: AppPolicies.CorsPolicy,
                    policy =>
                    {
                        if (allowedOrigins != null && allowedOrigins.Length > 0)
                        {
                            policy.WithOrigins(allowedOrigins)
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                                  //.AllowCredentials();
                        }
                    });
            });

            return services;
        }
    }
}
