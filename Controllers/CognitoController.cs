using Microsoft.AspNetCore.Mvc;
using SecureAuthDemo.Services;

namespace SecureAuthDemo.Controllers
{
    [ApiController]
    [Route("api/auth/aws")]
    public class CognitoController : Controller
    {
        private readonly ICognitoAuthService _cognitoAuthService;
        private readonly IAuthService _authService;

        public CognitoController(ICognitoAuthService cognitoAuthService, IAuthService authService)
        {
            _cognitoAuthService = cognitoAuthService;
            _authService = authService;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var url = _cognitoAuthService.GetLoginUrl();
            //return Redirect(url);
            return Ok(new { LoginUrl = url });
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            var userInfo = await _cognitoAuthService.GetUserInfoAsync(code);
            if (userInfo.Email == null)
                return BadRequest("Invalid Google token");

            // Create a JWT and refresh token for this user
            var tokens = await _authService.GenerateTokensForSSOUserAsync(userInfo.Email, userInfo.Name);

            return Ok(tokens);
        }
    }
}
