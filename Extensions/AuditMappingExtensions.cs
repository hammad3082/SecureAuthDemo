using SecureAuthDemo.Configuration;
using SecureAuthDemo.Repositories;
using SecureAuthDemo.Services.Auditing;

namespace SecureAuthDemo.Extensions
{
    public static class AuditMappingExtensions
    {
        public static IServiceCollection AddAuditServices(this IServiceCollection services, IConfiguration configuration) 
        {
            services.Configure<AuditSettings>(configuration.GetSection(nameof(AuditSettings)));

            services.AddSingleton<AuditLogQueue>();

            services.AddScoped<IAuditLogRepository, AuditLogRepository>();

            services.AddScoped<IAuditLogService, AuditLogService>();

            services.AddHostedService<AuditLogProcessor>();

            return services;
        }
    }
}