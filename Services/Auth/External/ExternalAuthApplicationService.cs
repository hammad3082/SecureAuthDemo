using SecureAuthDemo.Enums;
using SecureAuthDemo.Services.Auth.Abstractions;

namespace SecureAuthDemo.Services.Auth.External
{
    public class ExternalAuthApplicationService
    {
        private readonly IOauthStateCacheService _stateStore;
        private readonly IAuthService _authService;
        private readonly ExternalAuthServiceResolver _resolver;
        private readonly ILogger<ExternalAuthApplicationService> _logger;
        private static readonly TimeSpan StateTtl = TimeSpan.FromMinutes(5);

        public ExternalAuthApplicationService(
            ExternalAuthServiceResolver resolver,
            IOauthStateCacheService stateStore,
            IAuthService authService,
            ILogger<ExternalAuthApplicationService> logger)
        {
            _resolver = resolver;
            _stateStore = stateStore;
            _logger = logger;
            _authService = authService;
        }

        public async Task<string> StartLoginAsync(AuthProvider provider)
        {
            var state = Guid.NewGuid().ToString("N");
            _logger.LogInformation($"Starting login flow. Provider={provider}, State={state}");

            await _stateStore.SetStateAsync(state, provider, StateTtl);

            var service = _resolver.Get(provider);
            var url = service.GetLoginUrl(state);

            return url;
        }

        public async Task<(string accessToken, string refreshToken)> HandleCallbackAsync(string code, string state)
        {
            _logger.LogInformation("Processing callback. State={State}", state);

            if (string.IsNullOrWhiteSpace(state))
                throw new Exception("Missing state");

            var (found, provider) = await _stateStore.TryGetProviderAsync(state);
            if (!found)
                throw new Exception("Invalid or expired state");

            await _stateStore.RemoveStateAsync(state);
            _logger.LogInformation("State validated. Provider={Provider}", provider);

            var service = _resolver.Get(provider);

            try
            {
                var userInfo = await service.GetUserInfoAsync(code);

                if (userInfo.Email == null)
                    throw new Exception("Invalid token");

                // Create a JWT and refresh token for this user
                return await _authService.GenerateTokensForSSOUserAsync(userInfo.Email, userInfo.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Callback processing failed");
                throw new Exception("External login failed");
            }
        }
    }
}
