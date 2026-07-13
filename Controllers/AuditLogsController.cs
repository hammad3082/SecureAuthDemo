using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecureAuthDemo.Models;
using SecureAuthDemo.Services.Auditing;
using System.Security.Claims;

namespace SecureAuthDemo.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AuditLogsController(IAuditLogService _auditLogService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PaginatedEnvelope>> GetMyLogs([FromQuery] AuditLogQueryRequest request)
        {
            if (request.PageSize < 1 || request.PageSize > 100) request.PageSize = 10;

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out int currentUserId))
            {
                return Unauthorized(new { message = "User identity context is missing or invalid." });
            }

            var result = await _auditLogService.GetLogsByUserIdAsync(currentUserId, request);

            return Ok(result);
        }
    }
}
