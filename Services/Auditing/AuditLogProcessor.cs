namespace SecureAuthDemo.Services.Auditing
{
    public class AuditLogProcessor : BackgroundService
    {
        private readonly AuditLogQueue _queue;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AuditLogProcessor> _logger;

        public AuditLogProcessor(AuditLogQueue queue, IServiceProvider serviceProvider, ILogger<AuditLogProcessor> logger)
        {
            _queue = queue;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Audit Log Background Processor is starting...");

            await foreach (var log in _queue.DequeueAllLogsAsync(stoppingToken)) 
            {
                try
                {
                    await using (var scope = _serviceProvider.CreateAsyncScope())
                    {

                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }

            _logger.LogInformation("Audit Log Background Processor has shut down cleanly.");
        }
    }
}
