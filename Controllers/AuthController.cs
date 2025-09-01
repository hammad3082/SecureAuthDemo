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
            return "hello World";
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
    }
}
