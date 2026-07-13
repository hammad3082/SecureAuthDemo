using SecureAuthDemo.Entities;
using SecureAuthDemo.Models;

namespace SecureAuthDemo.Services.Auditing
{
    public interface IAuditLogService
    {
        Task ProcessAndQueueLogAsync(HttpContext context);
        Task SaveLogToDatabaseAsync(AuditLog log);
        Task<PaginatedEnvelope> GetLogsByUserIdAsync(int userId, AuditLogQueryRequest request);
    }
}
