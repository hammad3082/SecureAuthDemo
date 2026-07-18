namespace SecureAuthDemo.Models
{
    public record SessionDiagnosticsResponse(
        string UserName,
        string UserRole,
        string IpAddress,
        string UserAgent,
        string LoginMethod,
        bool IsTwoFactorEnabled
    );

    public record UserSecurityDetails(
        string UserName,
        string LoginProvider,
        bool IsTwoFactorEnabled
    );
}
