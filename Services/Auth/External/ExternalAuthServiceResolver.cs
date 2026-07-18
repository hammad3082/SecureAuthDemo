using SecureAuthDemo.Enums;
using SecureAuthDemo.Services.Auth.Abstractions;

namespace SecureAuthDemo.Services.Auth.External
{
    public class ExternalAuthServiceResolver
    {
        private readonly GoogleAuthService _google;
        private readonly CognitoAuthService _cognito;
        public ExternalAuthServiceResolver(GoogleAuthService google, CognitoAuthService cognito)
        {
            _google = google;
            _cognito = cognito;
        }

        public IExternalAuthService Get(AuthProvider provider)
        {
            switch (provider)
            {
                case AuthProvider.Google:
                    return _google;

                case AuthProvider.Cognito:
                    return _cognito;

                case AuthProvider.Local:
                    throw new InvalidOperationException("Local authentication does not use an external auth provider.");

                default:
                    throw new ArgumentOutOfRangeException(nameof(provider), "Unsupported provider");
            }

        }
    }
}
