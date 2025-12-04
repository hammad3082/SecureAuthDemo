using SecureAuthDemo.Enums;
using SecureAuthDemo.Services.Interfaces;

namespace SecureAuthDemo.Services.Implimentations
{
    public class ExternalAuthFlow
    {
        private readonly IStateStore _stateStore;
        private readonly IAuthService _authService;
        private readonly AuthServiceFactory _factory;
        private readonly ILogger<ExternalAuthFlow> _logger; 
        private static readonly TimeSpan StateTtl = TimeSpan.FromMinutes(5);

        public ExternalAuthFlow(
            AuthServiceFactory factory,
            IStateStore stateStore,
            IAuthService authService,
            ILogger<ExternalAuthFlow> logger)
        {
            _factory = factory;
            _stateStore = stateStore;
            _logger = logger;
            _authService = authService;
        }

        public async Task<string> StartLoginAsync(AuthProvider provider)
        {
            var state = Guid.NewGuid().ToString("N");
            _logger.LogInformation($"Starting login flow. Provider={provider}, State={state}");

            await _stateStore.SetStateAsync(state, provider, StateTtl);

            var service = _factory.Get(provider);
            var url = service.GetLoginUrl(state);

            return url;
        }

        public async Task<object> HandleCallbackAsync(string code, string state)
        {
            _logger.LogInformation("Processing callback. State={State}", state);

            if (string.IsNullOrWhiteSpace(state))
                return new Exception("Missing state");

            var (found, provider) = await _stateStore.TryGetProviderAsync(state);
            if (!found)
                return new Exception("Invalid or expired state");

            await _stateStore.RemoveStateAsync(state);
            _logger.LogInformation("State validated. Provider={Provider}", provider);

            var service = _factory.Get(provider);

            try
            {
                var userInfo = await service.GetUserInfoAsync(code);

                if (userInfo.Email == null)
                    return new Exception("Invalid token");

                // Create a JWT and refresh token for this user
                return await _authService.GenerateTokensForSSOUserAsync(userInfo.Email, userInfo.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Callback processing failed");
                return new Exception("External login failed");
            }
        }
    }
}
