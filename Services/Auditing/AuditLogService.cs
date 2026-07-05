using SecureAuthDemo.Models;
using SecureAuthDemo.Repositories;

namespace SecureAuthDemo.Services.Auditing
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogService(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        public async Task SaveLogToDatabaseAsync(AuditLog log)
        {
            if (log == null) return;

            await _auditLogRepository.AddAsync(log);
            await _auditLogRepository.SaveChangesAsync();
        }
    }
}
