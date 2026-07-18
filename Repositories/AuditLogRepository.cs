using Microsoft.EntityFrameworkCore;
using SecureAuthDemo.Data;
using SecureAuthDemo.Entities;
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

        public async Task<PaginatedEnvelope> GetLogsByUserIdAsync(int userId, AuditLogQueryRequest request)
        {
            var query = _db.AuditLogs
                .Where(log => log.UserId == userId)
                .OrderByDescending(log => log.Timestamp);

            var totalCount = await query.CountAsync();

            var logs = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(log => new AuditLogResponse
                {
                    Id = log.Id,
                    EventType = log.EventType,
                    RequestMethod = log.RequestMethod,
                    RequestPath = log.RequestPath,
                    StatusCode = log.StatusCode,
                    IpAddress = log.IpAddress,
                    Timestamp = log.Timestamp
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new PaginatedEnvelope
            {
                Items = logs,
                PageNumber = request.Page,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}
