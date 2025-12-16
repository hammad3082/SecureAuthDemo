namespace SecureAuthDemo.Services.Auth.Abstractions
{
    public interface IExternalAuthService
    {
        string GetLoginUrl(string state);
        Task<(string? Email, string? Name)> GetUserInfoAsync(string authorizationCode);
    }
}
