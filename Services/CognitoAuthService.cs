
using Microsoft.Extensions.Options;
using SecureAuthDemo.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Google.Apis.Auth.OAuth2.Responses;
using System.IdentityModel.Tokens.Jwt;

namespace SecureAuthDemo.Services
{
    public class CognitoAuthService : ICognitoAuthService
    {

        private readonly HttpClient _httpClient;
        private readonly AwsCognitoSettings _awsCognitoSettings;

        public CognitoAuthService(IOptions<AwsCognitoSettings> settings)
        {
            _awsCognitoSettings = settings.Value;
            _httpClient = new HttpClient();
        }

        public string GetLoginUrl()
        {
            var domain = _awsCognitoSettings.Domain;
            var clientId = _awsCognitoSettings.ClientId;
            var redirectUri = Uri.EscapeDataString(_awsCognitoSettings.RedirectUri);

            return $"{domain}/login" +
                $"?client_id={clientId}" +
                $"&response_type=code" +
                $"&scope=email+openid+phone" +
                $"&redirect_uri={redirectUri}";
        }
        public async Task<(string? Email, string? Name)> GetUserInfoAsync(string code)
        {
            var domain = _awsCognitoSettings.Domain;
            var clientId = _awsCognitoSettings.ClientId;
            var clientSecret = _awsCognitoSettings.ClientSecret;
            var redirectUri = _awsCognitoSettings.RedirectUri;

            var tokenUrl = $"{domain}/oauth2/token";

            using var client = new HttpClient();

            // Basic Authorization header
            var authString = $"{clientId}:{clientSecret}";
            var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes(authString));
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", authHeader);

            // Form body
            var form = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", redirectUri }
            };

            var response = await client.PostAsync(tokenUrl, new FormUrlEncodedContent(form));
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Token request failed: {json}");

            using var doc = JsonDocument.Parse(json);

            var idToken = doc.RootElement.GetProperty("id_token").GetString();

            // decode JWT using JwtSecurityTokenHandler
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(idToken);

            var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = jwt.Claims.FirstOrDefault(c => c.Type == "cognito:username")?.Value;

            return (email, name);
        }

    }
}
