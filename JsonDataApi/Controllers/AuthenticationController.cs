using JsonDataApi.Models;
using JsonDataApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace JsonDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticateController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        // POST: api/authenticate/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var result = await _authenticationService.Register(user);

            if (result == "User already exists.")
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // POST: api/authenticate/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            var result = await _authenticationService.Login(user);

            if (result == "Invalid email or password.")
            {
                return Unauthorized(result);
            }

            return Ok(new { token = result });
        }
    }
}
