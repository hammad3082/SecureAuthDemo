using SecureAuthDemo.Models;

namespace SecureAuthDemo.Services
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request);
        Task<(string accessToken, string refreshToken)> LoginAsync(LoginRequest request);
        Task<bool> ValidateUserAsync(string username, string password);
        public Task<string> RefreshTokenAsync(string refreshToken);
        Task<object> GenerateTokensForSSOUserAsync(string email, string name);
    }
}
