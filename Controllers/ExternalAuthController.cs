using Microsoft.AspNetCore.Mvc;
using SecureAuthDemo.Enums;
using SecureAuthDemo.Services;
using SecureAuthDemo.Services.Implimentations;
using System;

namespace SecureAuthDemo.Controllers
{
    [ApiController]
    [Route("api/auth/external")]
    public class ExternalAuthController : ControllerBase
    {
        private readonly ExternalAuthFlow _flow;
        public ExternalAuthController(ExternalAuthFlow flow)
        {
            _flow = flow;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login([FromQuery] AuthProvider provider)
        {
            var url = await _flow.StartLoginAsync(provider);

            //return Redirect(url);
            return Ok(new { LoginUrl = url });
        }

        [HttpGet]
        //[HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            var tokens = await _flow.HandleCallbackAsync(code, state);
            
            return Ok(tokens);
        }
    }
}
