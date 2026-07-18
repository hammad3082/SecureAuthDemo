using SecureAuthDemo.Services.Auditing;

namespace SecureAuthDemo.Middleware
{
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditLogMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuditLogService auditLogService)
        {
            await _next(context);

            await auditLogService.ProcessAndQueueLogAsync(context);
        }
    }
}
