using Microsoft.EntityFrameworkCore;
using SecureAuthDemo.Data;

namespace SecureAuthDemo.Services
{
    public class HealthService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<HealthService> _logger;
        public HealthService(AppDbContext db, ILogger<HealthService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<bool> RunWarmupDiagnosticsAsync()
        {
            try
            {
                _logger.LogInformation("WARMUP: Start Of DB Exec");
                await _db.Database.ExecuteSqlRawAsync("SELECT 1;");
                _logger.LogInformation("WARMUP: End Of DB Exec");

                await _db.Users.AnyAsync(u => u.Username == "warmup_ping");
                _logger.LogInformation("WARMUP: End Of User DB Exec");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("DB Start-Up Error: ", ex.Message);
                return false;
            }
        }
    }
}
