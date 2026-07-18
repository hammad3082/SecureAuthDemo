using Microsoft.AspNetCore.HttpOverrides;
using SecureAuthDemo.Repositories;
using SecureAuthDemo.Services;
using SecureAuthDemo.Services.Auth.Abstractions;
using SecureAuthDemo.Services.Auth.External;
using SecureAuthDemo.Services.Auth.Local;
using SecureAuthDemo.Services.Auth.State;
using SecureAuthDemo.Services.Infrastructure;
using SecureAuthDemo.Services.Security;

namespace SecureAuthDemo.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<HealthService>();

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            // Auth Services
            services.AddScoped<IAuthService, LocalAuthService>();
            services.AddScoped<IOauthStateCacheService, OauthStateCacheService>();
            services.AddScoped<ExternalAuthApplicationService>();

            services.AddHttpClient();

            // External Providers
            services.AddTransient<GoogleAuthService>();
            services.AddTransient<CognitoAuthService>();
            services.AddSingleton<ExternalAuthServiceResolver>();

            services.AddScoped<ISecurityService, SecurityService>();

            return services;
        }

        public static IServiceCollection AddProxyHeadersConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

                if (configuration.GetValue<bool>("ForwardedHeaders:TrustAllProxies"))
                {
                    options.KnownProxies.Clear();
                    options.KnownNetworks.Clear();
                }
            });

            return services;
        }

        public static IServiceCollection AddUserContextAccessor(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<UserContextAccessor>();

            return services;
        }
    }
}
