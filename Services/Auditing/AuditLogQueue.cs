using System.Linq;
using System.Collections.Generic;
using SecureAuthDemo.Models;
using System.Threading.Channels;
using SecureAuthDemo.Configuration;
using Microsoft.Extensions.Options;

namespace SecureAuthDemo.Services.Auditing
{
    public class AuditLogQueue
    {
        private readonly Channel<AuditLog> _channel;
        private readonly AuditSettings _auditSettings;
        private readonly ILogger<AuditLogQueue> _logger;

        public AuditLogQueue(IOptions<AuditSettings> auditSetting, ILogger<AuditLogQueue> logger)
        {
            _auditSettings = auditSetting.Value;
            _logger = logger;

            if (_auditSettings.IsEnabled)
            {
                var options = new BoundedChannelOptions(10000)
                {
                    FullMode = BoundedChannelFullMode.Wait
                };

                _channel = Channel.CreateBounded<AuditLog>(options);
            }
        }

        public async ValueTask QueueBackgroundLogAsync(AuditLog log)
        {
            if(!_auditSettings.IsEnabled || log == null || _channel == null) return;

            try
            {
                if (!_channel.Writer.TryWrite(log))
                {
                    _logger.LogInformation("Audit log queue is full. Attempting timed asynchronous write.");

                    // Token that auto-cancels after 50 milliseconds
                    using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

                    await _channel.Writer.WriteAsync(log, cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogError("Dropped audit log: Asynchronous write timed out after 50ms because the queue buffer is full.");
            }
            catch (ChannelClosedException ex)
            {
                _logger.LogError(ex, "Failed to queue audit log: The channel writer has been closed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception occurred while trying to queue an audit log.");
            }
            finally
            {
                _logger.LogInformation("Finished attempt to queue audit log for tracking.");
            }
        }

        public IAsyncEnumerable<AuditLog> DequeueAllLogsAsync(CancellationToken cancellationToken)
        {
            if (!_auditSettings.IsEnabled || _channel == null)
            {
                return GetEmptyStream();
            }

            return _channel.Reader.ReadAllAsync(cancellationToken);
        }

        public void StopQueue()
        {
            _channel?.Writer.TryComplete();
        }

        private static async IAsyncEnumerable<AuditLog> GetEmptyStream()
        {
            yield break;
        }
    }
}
