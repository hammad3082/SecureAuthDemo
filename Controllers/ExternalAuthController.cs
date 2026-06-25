using Microsoft.AspNetCore.Mvc;
using SecureAuthDemo.Enums;
using SecureAuthDemo.Services;
using SecureAuthDemo.Services.Auth.External;
using System;

namespace SecureAuthDemo.Controllers
{
    [ApiController]
    [Route("api/auth/external")]
    public class ExternalAuthController : ControllerBase
    {
        private readonly ExternalAuthApplicationService _flow; 
        private readonly IConfiguration _config;
        public ExternalAuthController(ExternalAuthApplicationService flow, IConfiguration config)
        {
            _flow = flow;
            _config = config;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery] AuthProvider provider)
        {
            var url = await _flow.StartLoginAsync(provider);

            //return Redirect(url);
            return Ok(new { LoginUrl = url });
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            var tokens = await _flow.HandleCallbackAsync(code, state);

            string baseRedirectUrl = _config["Authentication:ClientRedirectUrl"]
                                        ?? "http://localhost:4200/auth/callback";

            string angularCallbackUrl = $"{baseRedirectUrl}?accessToken={tokens.accessToken}&refreshToken={tokens.refreshToken}";

            return Redirect(angularCallbackUrl);
        }
    }
}
