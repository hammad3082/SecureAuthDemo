using SecureAuthDemo.Enums;
using System.ComponentModel.DataAnnotations;

namespace SecureAuthDemo.Models
{
    public class AuditLog
    {
        public long Id { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        // Core tracking fields
        [MaxLength(50)]
        public string EventType { get; set; } = string.Empty;//"Login.Success", "API.Access"
        
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;//"User logged in via Google SSO"

        // AI Optimization Fields (Tracking specific API footprint patterns)
        [MaxLength(10)]
        public string RequestMethod { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string RequestPath { get; set; } = string.Empty;
        public int StatusCode { get; set; }

        // Client metadata
        [MaxLength(45)]
        public string IpAddress { get; set; } = string.Empty;
        
        [MaxLength(1000)] 
        public string UserAgent { get; set; } = string.Empty;//Captures browser and OS signatures
        
        public AuditLogPriority Priority { get; set; } = AuditLogPriority.Medium;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
