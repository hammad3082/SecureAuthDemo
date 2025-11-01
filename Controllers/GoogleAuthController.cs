using Microsoft.AspNetCore.Mvc;
using SecureAuthDemo.Services;

namespace SecureAuthDemo.Controllers
{
    [ApiController]
    [Route("api/auth/google")]
    public class GoogleAuthController(IGoogleAuthService googleAuthService, IAuthService authService) : ControllerBase
    {
        private readonly IGoogleAuthService _googleAuthService = googleAuthService ;
        private readonly IAuthService _authService = authService;

        [HttpGet("login")]
        public IActionResult Login()
        {
            var url = _googleAuthService.GetGoogleLoginUrl();
            return Ok(new { LoginUrl = url });
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            var userInfo = await _googleAuthService.GetUserInfoAsync(code);
            if (userInfo.Email == null)
                return BadRequest("Invalid Google token");

            // Create a JWT and refresh token for this user
            var tokens = await _authService.GenerateTokensForGoogleUserAsync(userInfo.Email, userInfo.Name);

            return Ok(tokens);
        }
    }
}
