namespace SecureAuthDemo.Configuration
{
    public class GoogleAuthSettings
    {
        public string ClientId { get; set; } = "";
        public string ClientSecret { get; set; } = "";
        public string RedirectUri { get; set; } = "";
        public string TokenUri { get; set; } = "";
    }
}
