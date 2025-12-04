using SecureAuthDemo.Enums;

namespace SecureAuthDemo.Services
{
    public class AuthServiceFactory
    {
        private readonly GoogleAuthService _google;
        private readonly CognitoAuthService _cognito;
        public AuthServiceFactory(GoogleAuthService google, CognitoAuthService cognito)
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

                default:
                    throw new ArgumentOutOfRangeException(nameof(provider), "Unsupported provider");
            }

        }
    }
}
