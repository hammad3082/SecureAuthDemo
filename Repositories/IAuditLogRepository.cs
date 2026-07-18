using SecureAuthDemo.Entities;
using SecureAuthDemo.Models;

namespace SecureAuthDemo.Repositories
{
    public interface IAuditLogRepository
    {
        Task AddAsync(AuditLog log);
        Task SaveChangesAsync();
        Task<PaginatedEnvelope> GetLogsByUserIdAsync(int userId, AuditLogQueryRequest request);
    }
}
