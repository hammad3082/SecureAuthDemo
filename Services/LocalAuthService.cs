
using Microsoft.AspNetCore.Identity;
using SecureAuthDemo.Models;
using SecureAuthDemo.Repositories;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SecureAuthDemo.Configuration;
using Microsoft.Extensions.Options;

namespace SecureAuthDemo.Services
{
    public class LocalAuthService : IAuthService
    {

        private readonly IUserRepository _userRepo;
        private readonly JwtSettings _jwtSettings;
        public LocalAuthService(IUserRepository userRepo, IOptions<JwtSettings> jwtOptions)
        {
            _userRepo = userRepo;
            _jwtSettings = jwtOptions.Value;
        }
        public async Task RegisterAsync(RegisterRequest request)
        {
            try
            {
                var exists = await _userRepo.GetByUsernameAsync(request.Username);
                if (exists != null)
                    throw new Exception("User Already Exists");

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                User newUser = new User
                {
                    Username = request.Username,
                    PasswordHash = hashedPassword,
                    Email = request.Email,
                    CreatedAt = DateTime.UtcNow,
                };

                await _userRepo.AddAsync(newUser);
                await _userRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var user = await _userRepo.GetByUsernameAsync(username);
            if (user != null)
                return false;

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public async Task<string> LoginAsync(LoginRequest request)
        {
            var user = await _userRepo.GetByUsernameAsync(request.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception("Invalid username or password");
            }

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);

            var claims = new List<Claim>
            {
                 new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var RoleClaim = new Claim("Role", "Admin");

            claims.Add(RoleClaim);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
