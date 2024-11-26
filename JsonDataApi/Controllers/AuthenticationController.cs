using JsonDataApi.Models;
using JsonDataApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace JsonDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticateController> _logger;

        public AuthenticateController(IAuthenticationService authenticationService, ILogger<AuthenticateController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        // POST: api/authenticate/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "Invalid user data." });
            }

            try
            {
                var result = await _authenticationService.Register(user);

                if (result == "User already exists.")
                {
                    return BadRequest(new { message = result });
                }

                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        // POST: api/authenticate/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            try
            {
                var result = await _authenticationService.Login(user);

                if (result == "Invalid email or password.")
                {
                    return Unauthorized(new { message = result });
                }

                return Ok(new { token = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
