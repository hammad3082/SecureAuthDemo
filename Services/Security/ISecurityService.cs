using SecureAuthDemo.Models;

namespace SecureAuthDemo.Services.Security
{
    public interface ISecurityService
    {
        Task<UserSecurityDetails> GetAccountSecurityDetailsAsync(int userId);
    }
}
