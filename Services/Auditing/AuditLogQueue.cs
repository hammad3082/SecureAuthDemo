using System.Linq;
using System.Collections.Generic;
using SecureAuthDemo.Models;
using System.Threading.Channels;

namespace SecureAuthDemo.Services.Auditing
{
    public class AuditLogQueue
    {
        private readonly Channel<AuditLog> _channel; 
        private readonly bool _isEnabled;

        public AuditLogQueue(IConfiguration configuration)
        {
            _isEnabled = configuration.GetValue<bool>("AuditSettings:IsEnabled");

            if (_isEnabled)
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
            if(!_isEnabled || log == null || _channel == null) return;

            try
            {
                if (!_channel.Writer.TryWrite(log))
                {
                    await _channel.Writer.WriteAsync(log);
                }
            }
            catch (ChannelClosedException)
            {
                // Catch safely if the channel writer was completed mid-execution
            }
        }

        public IAsyncEnumerable<AuditLog> DequeueAllLogsAsync(CancellationToken cancellationToken)
        {
            if (!_isEnabled || _channel == null)
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
