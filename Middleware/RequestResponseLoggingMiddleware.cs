namespace SecureAuthDemo.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;

            _logger.LogInformation($"Incoming Request: {context.Request.Method} {context.Request.Path} at [{startTime}]");

            await _next(context);

            var endTime = DateTime.UtcNow;
            var duration = endTime - startTime;

            _logger.LogInformation($"Response: {context.Response.StatusCode} for {context.Request.Method} {context.Request.Path} completed at [{endTime}] (Duration: {duration.TotalMilliseconds} ms)");          
        }
    }
}
