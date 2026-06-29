using Microsoft.AspNetCore.Mvc;
using SecureAuthDemo.Services;

namespace SecureAuthDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController(HealthService healthService) : ControllerBase
    {
        [HttpGet("warmup")]
        public async Task<IActionResult> TriggerWarmup()
        {
            bool isReady = await healthService.RunWarmupDiagnosticsAsync();
            
            if (!isReady)
            {
                return StatusCode(503, new { status = "Unhealthy" });
            }

            return Ok(new { status = "Healthy", warmed = true });
        }
    }
}
