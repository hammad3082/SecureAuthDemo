using SecureAuthDemo.Models;

namespace SecureAuthDemo.Services.Auditing
{
    public interface IAuditLogService
    {
        Task SaveLogToDatabaseAsync(AuditLog log);
    }
}
