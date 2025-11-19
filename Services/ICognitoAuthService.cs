namespace SecureAuthDemo.Services
{
    public interface ICognitoAuthService
    {
        string GetLoginUrl();
        Task<(string? Email, string? Name)> GetUserInfoAsync(string code);
    }
}
