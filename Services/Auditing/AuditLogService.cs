using Microsoft.Extensions.Options;
using SecureAuthDemo.Configuration;
using SecureAuthDemo.Constants;
using SecureAuthDemo.Enums;
using SecureAuthDemo.Models;
using SecureAuthDemo.Repositories;
using System.Security.Claims;

namespace SecureAuthDemo.Services.Auditing
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly AuditSettings _auditSettings;
        private readonly AuditLogQueue _queue;
        private readonly ILogger<IAuditLogService> _logger;

        public AuditLogService(IAuditLogRepository auditLogRepository, IOptions<AuditSettings> options, AuditLogQueue queue, ILogger<IAuditLogService> logger)
        {
            _auditLogRepository = auditLogRepository;
            _auditSettings = options.Value;
            _queue = queue;
            _logger = logger;
        }

        public async Task ProcessAndQueueLogAsync(HttpContext context)
        {
            _logger.LogInformation("Start- Constructing of Audit Log Object");

            var method = context.Request.Method;
            var statusCode = context.Response.StatusCode;
            var path = context.Request.Path.Value ?? string.Empty;

            var rules = EvaluatePipelineMetrics(method, statusCode, path);
            if (rules == null) return;

            if (rules.Value.Priority < _auditSettings.MinimumPriority) return;

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = context.User.Identity?.Name ?? "Anonymous";
            var userAgent = context.Request.Headers["User-Agent"].ToString();

            var log = new AuditLog
            {
                EventType = rules.Value.EventType,
                Description = $"User '{username}' triggered {method} on {path}. Status: {statusCode}",
                RequestMethod = method,
                RequestPath = path,
                StatusCode = statusCode,
                IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                UserAgent = string.IsNullOrEmpty(userAgent) ? "Unknown" : userAgent,
                Priority = rules.Value.Priority,
                UserId = string.IsNullOrEmpty(userIdClaim) ? null : int.Parse(userIdClaim)
            };

            _logger.LogInformation("Passing Audit log to Queue");

            await _queue.QueueBackgroundLogAsync(log);
        }

        public async Task SaveLogToDatabaseAsync(AuditLog log)
        {
            if (log == null) return;

            await _auditLogRepository.AddAsync(log);
            await _auditLogRepository.SaveChangesAsync();
        }

        private (AuditLogPriority Priority, string EventType)? EvaluatePipelineMetrics(string method, int statusCode, string path)
        {
            if (statusCode == StatusCodes.Status401Unauthorized || statusCode == StatusCodes.Status403Forbidden)
            {
                return (AuditLogPriority.Critical, AuditEventTypes.AuthFailure);
            }

            if (method == HttpMethods.Delete)
            {
                return (AuditLogPriority.High, AuditEventTypes.ApiAccess);
            }

            if (method == HttpMethods.Post || method == HttpMethods.Put)
            {
                return (AuditLogPriority.Medium, AuditEventTypes.ApiAccess);
            }

            if (method == HttpMethods.Get)// && path.StartsWith("/api/admin", StringComparison.OrdinalIgnoreCase))
            {
                return (AuditLogPriority.Low, AuditEventTypes.AdminAccess);
            }

            return null;
        }

    }
}
