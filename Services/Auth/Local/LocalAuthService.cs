
using Azure.Core;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SecureAuthDemo.Configuration;
using SecureAuthDemo.Middleware;
using SecureAuthDemo.Models;
using SecureAuthDemo.Repositories;
using SecureAuthDemo.Services.Auth.Abstractions;
using SecureAuthDemo.Services.Cache;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SecureAuthDemo.Services.Auth.Local
{
    public class LocalAuthService : IAuthService
    {

        private readonly IUserRepository _userRepo;
        private readonly ICacheService _cacheService;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<LocalAuthService> _logger;
        public LocalAuthService(IUserRepository userRepo, IOptions<JwtSettings> jwtOptions, ICacheService cacheService, ILogger<LocalAuthService> logger)
        {
            _userRepo = userRepo;
            _jwtSettings = jwtOptions.Value;
            _cacheService = cacheService;
            _logger = logger;
        }
        public async Task RegisterAsync(RegisterRequest request)
        {
            try
            {
                var exists = await _userRepo.GetByEmailAsync(request.Email);
                if (exists != null)
                    throw new Exception("Email Already Exists");

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
        public async Task<(string accessToken, string refreshToken)> LoginAsync(LoginRequest request)
        {
            _logger.LogInformation("[START of Login], Checking for User in DB");
            var user = await _userRepo.GetByEmailAsync(request.Email);

            _logger.LogInformation("Got DB User Check Response");
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new Exception("Invalid username or password");
            }

            _logger.LogInformation("Generating JWT Token");
            var accessToken = GenerateJwtToken(user);
            var refreshToken = Guid.NewGuid().ToString();

            _logger.LogInformation("Setting Refresh Token to Cache");
            await _cacheService.SetAsync(refreshToken, user.Id.ToString(), TimeSpan.FromDays(7));

            _logger.LogInformation("[END of Login]");
            return (accessToken, refreshToken);
        }
        public async Task<string> RefreshTokenAsync(string refreshToken)
        {
            var userId = await _cacheService.GetAsync(refreshToken);
            if (userId == null)
                throw new Exception("Invalid or expired refresh token");

            var user = await _userRepo.GetByIdAsync(Convert.ToInt32(userId));
            if (user == null)
                throw new Exception("User not found");

            var newAccessToken = GenerateJwtToken(user);

            return newAccessToken;
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
        public async Task<(string accessToken, string refreshToken)> GenerateTokensForSSOUserAsync(string email, string name)
        {
            //var user = await _userRepo.GetByEmailAsync(email);
            var user = await _userRepo.GetByEmailAsync(email);
            _logger.LogInformation("Got user From DB");

            if (user == null)
            {
                _logger.LogInformation("Creating new user to DB");
                user = new User
                {
                    Username = name,
                    Email = email,
                    PasswordHash = "",
                    CreatedAt = DateTime.UtcNow,
                    //Role = "User"
                };

                await _userRepo.AddAsync(user);
                await _userRepo.SaveChangesAsync();

                // To get userID
                user = await _userRepo.GetByEmailAsync(email);
            }

            _logger.LogInformation("Create JWT Token");
            var accessToken = GenerateJwtToken(user);

            var refreshToken = Guid.NewGuid().ToString();

            _logger.LogInformation("Set refreshToken to redis");
            await _cacheService.SetAsync(refreshToken, user.Id.ToString(), TimeSpan.FromDays(7));

            _logger.LogInformation("Returning Tokens");
            return (accessToken, refreshToken);
        }
    }
}
