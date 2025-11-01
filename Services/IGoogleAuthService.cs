namespace SecureAuthDemo.Services
{
    public interface IGoogleAuthService
    {
        string GetGoogleLoginUrl();
        Task<(string? Email, string? Name)> GetUserInfoAsync(string authorizationCode);
    }
}
