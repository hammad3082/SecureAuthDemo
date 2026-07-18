using SecureAuthDemo.Models;
using SecureAuthDemo.Repositories;

namespace SecureAuthDemo.Services.Security
{
    public class SecurityService : ISecurityService
    {
        private readonly IUserRepository _userRepo;
        public SecurityService(IUserRepository userRepository) 
        {
            _userRepo = userRepository;
        }

        public async Task<UserSecurityDetails> GetAccountSecurityDetailsAsync(int userId)
        {
            return await _userRepo.GetSecurityDetailsByIdAsync(userId);
        }

    }
}
