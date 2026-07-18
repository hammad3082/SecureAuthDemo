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

            try
            {
                await foreach (var log in _queue.DequeueAllLogsAsync(stoppingToken))
                {
                    try
                    {
                        await using (var scope = _serviceProvider.CreateAsyncScope())
                        {
                            var auditService = scope.ServiceProvider.GetRequiredService<IAuditLogService>();
                            await auditService.SaveLogToDatabaseAsync(log);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to save individual audit log to the database. Continuing processing loop.");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Audit Log Background Processor cancellation requested.");
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Catastrophic error occurred in the Audit Log Background Processor loop!");
            }
            finally
            {
                _logger.LogInformation("Audit Log Background Processor has shut down cleanly.");
            }
        }
    }
}
