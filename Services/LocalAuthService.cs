
using Microsoft.AspNetCore.Identity;
using SecureAuthDemo.Models;
using SecureAuthDemo.Repositories;
using BCrypt.Net;

namespace SecureAuthDemo.Services
{
    public class LocalAuthService : IAuthService
    {

        private readonly IUserRepository _userRepo;
        public LocalAuthService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
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
    }
}
