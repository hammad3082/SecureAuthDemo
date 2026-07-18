using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecureAuthDemo.Entities;
using SecureAuthDemo.Models;
using SecureAuthDemo.Services.Security;
using System.Security.Claims;

namespace SecureAuthDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SecurityController(ISecurityService _securityService) : ControllerBase
    {
        [HttpGet("diagnostics")]
        public async Task<IActionResult> GetSessionDiagnostics()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //var userRole = User.FindFirstValue(ClaimTypes.Role);
            var userRole = User.FindFirstValue("Role");

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = Request.Headers["User-Agent"].ToString();

            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("User identity is invalid.");
            }

            UserSecurityDetails securityDetails = await _securityService.GetAccountSecurityDetailsAsync(userId);
            if (securityDetails == null)
            {
                return NotFound("User account details not found.");
            }

            var response = new SessionDiagnosticsResponse
            (
                securityDetails.UserName,
                userRole,
                ipAddress,
                userAgent,
                securityDetails.LoginProvider,
                securityDetails.IsTwoFactorEnabled
            );

            return Ok(response);
        }
    }
}
