using JsonDataApi.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JsonDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration Configuration;

        public AuthenticateController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            Configuration = configuration;
        }

        // POST: api/authenticate/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // Check if the user already exists
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return BadRequest("User already exists.");
            }

            // Hash the password before storing it
            var hashedPassword = HashPassword(user.Password);
            user.Password = hashedPassword;

            // Save user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        // POST: api/authenticate/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
             var serverSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:ServerSecret"]));
            // Find the user by email
            var userFound = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (userFound == null || !VerifyPassword(user.Password, userFound.Password))
            {
                return Unauthorized("Invalid email or password.");
            }

            // Generate JWT token on successful login
            var token = GenerateToken(serverSecret,userFound);

            return Ok(new { token });
        }

        // Method to hash the password using BCrypt
        private string HashPassword(string password)
        {
            // Hash the password using BCrypt
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Method to verify if the entered password matches the hashed password
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Use BCrypt to verify the password
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
        }

        // Method to generate JWT token without using claims
        private string GenerateToken(SecurityKey key, User user)
        { 
            var now = DateTime.Now;
            var issuer = Configuration["JWT:Issuer"];
            var audience = Configuration["JWT:Audience"];
             var identity = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Email, user.Email),
            });
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(issuer, audience, identity,
                now, now.Add(TimeSpan.FromHours(1)), now, signingCredentials);
            var encodedJwt = handler.WriteToken(token);
            return encodedJwt;
        }
    }
}
