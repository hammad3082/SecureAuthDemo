using SecureAuthDemo.Repositories;
using SecureAuthDemo.Services.Auth.Abstractions;
using SecureAuthDemo.Services.Auth.External;
using SecureAuthDemo.Services.Auth.Local;
using SecureAuthDemo.Services.Auth.State;

namespace SecureAuthDemo.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
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

            return services;
        }
    }
}
