using Microsoft.AspNetCore.Mvc;
using SecureAuthDemo.Models;
using SecureAuthDemo.Services;

namespace SecureAuthDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService authService)
        {
            _auth = authService;
        }

        [HttpGet]
        public string Get()
        {
            return "Hello World";
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                await _auth.RegisterAsync(request);
                return Ok(new { message = "Registered" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var tokens = await _auth.LoginAsync(request);

                return Ok(new { accessToken = tokens.accessToken, refreshToken = tokens.refreshToken });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var newAccessToken = await _auth.RefreshTokenAsync(request.RefreshToken);
            return Ok(new { accessToken = newAccessToken });
        }
    }
}
