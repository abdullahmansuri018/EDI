using JsonDataApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JsonDataApi.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> Register(User user)
        {
            // Check if the user already exists
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                return "User already exists.";
            }

            // Hash the password before storing it
            var hashedPassword = HashPassword(user.Password);
            user.Password = hashedPassword;

            // Save user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "User registered successfully.";
        }

        public async Task<string> Login(User user)
        {
            var serverSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:ServerSecret"]));

            // Find the user by email
            var userFound = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (userFound == null || !VerifyPassword(user.Password, userFound.Password))
            {
                return "Invalid email or password.";
            }

            // Generate JWT token on successful login
            var token = GenerateToken(serverSecret, userFound);

            return token;
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

        // Method to generate JWT token
        private string GenerateToken(SecurityKey key, User user)
        {
            var now = DateTime.Now;
            var issuer = _configuration["JWT:Issuer"];
            var audience = _configuration["JWT:Audience"];
            var identity = new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Id", user.Id.ToString())
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
