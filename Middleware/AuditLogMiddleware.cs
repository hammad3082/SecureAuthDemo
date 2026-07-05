using SecureAuthDemo.Services.Auditing;

namespace SecureAuthDemo.Middleware
{
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AuditLogQueue _queue;

        public AuditLogMiddleware(RequestDelegate next, AuditLogQueue queue)
        {
            _next = next;
            _queue = queue;
        }
    }
}
