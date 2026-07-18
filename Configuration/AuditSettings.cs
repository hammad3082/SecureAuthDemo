using SecureAuthDemo.Enums;

namespace SecureAuthDemo.Configuration
{
    public class AuditSettings
    {
        public const string SectionName = "AuditSettings";
        public bool IsEnabled { get; set; } = false;
        public AuditLogPriority MinimumPriority { get; set; } = AuditLogPriority.Low;
    }
}
