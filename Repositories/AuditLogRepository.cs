using SecureAuthDemo.Data;
using SecureAuthDemo.Models;

namespace SecureAuthDemo.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _db;
        public AuditLogRepository(AppDbContext db) => _db = db;
        
        public async Task AddAsync(AuditLog log)
        {
            await _db.AuditLogs.AddAsync(log);
        }
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
