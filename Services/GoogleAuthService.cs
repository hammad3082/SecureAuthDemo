
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using SecureAuthDemo.Configuration;
using System.Text.Json;

namespace SecureAuthDemo.Services
{
    public class GoogleAuthService : IExternalAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly GoogleAuthSettings _googleAuthSettings;

        public GoogleAuthService(IOptions<GoogleAuthSettings> settings)
        {
            _googleAuthSettings = settings.Value;
            _httpClient = new HttpClient();
        }
        public string GetLoginUrl(string state)
        {
            var url = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                      $"client_id={_googleAuthSettings.ClientId}" +
                      $"&redirect_uri={_googleAuthSettings.RedirectUri}" +
                      $"&response_type=code" +
                      $"&scope=openid%20email%20profile" +
                      $"&access_type=offline" +
                      $"&state={state}";

            return url;
        }

        public async Task<(string? Email, string? Name)> GetUserInfoAsync(string authorizationCode)
        {
            // Step 1: Exchange code for tokens
            var tokenRequest = new Dictionary<string, string>
            {
                ["code"] = authorizationCode,
                ["client_id"] = _googleAuthSettings.ClientId,
                ["client_secret"] = _googleAuthSettings.ClientSecret,
                ["redirect_uri"] = _googleAuthSettings.RedirectUri,
                ["grant_type"] = "authorization_code"
            };

            var response = await _httpClient.PostAsync(
                _googleAuthSettings.TokenUri,
                new FormUrlEncodedContent(tokenRequest)
            );

            response.EnsureSuccessStatusCode();
            var tokenResponse = await response.Content.ReadFromJsonAsync<JsonElement>();
            var idToken = tokenResponse.GetProperty("id_token").GetString();

            // Step 2: Validate and decode the token
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken!);

            return (payload.Email, payload.Name);
        }
    }
}
