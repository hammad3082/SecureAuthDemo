using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecureAuthDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        [HttpGet("public")]
        public IActionResult PublicEndpoint()
        {
            return Ok("This is a public endpoint");
        }

        [Authorize]
        [HttpGet("protected")]
        public IActionResult ProtectedEndpoint()
        {
            var username = User.Identity?.Name;
            return Ok($"Hello {username}, you accessed a protected endpoint");
        }
    }
}
