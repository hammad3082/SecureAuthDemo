using SecureAuthDemo.Models;

namespace SecureAuthDemo.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request);
        Task<bool> ValidateUserAsync(string username, string password);
    }
}
