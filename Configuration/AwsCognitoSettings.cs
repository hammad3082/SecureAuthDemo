namespace SecureAuthDemo.Configuration
{
    public class AwsCognitoSettings
    {
        public string Domain { get; set; } = "";
        public string ClientId { get; set; } = "";
        public string ClientSecret { get; set; } = "";
        public string RedirectUri { get; set; } = "";
    }
}
