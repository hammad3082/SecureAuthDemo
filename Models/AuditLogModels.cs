using System.ComponentModel.DataAnnotations;

namespace SecureAuthDemo.Models
{
    public class AuditLogQueryRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be 1 or greater.")]
        public int Page { get; set ; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class AuditLogResponse
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string RequestMethod { get; set; } = string.Empty;
        public string RequestPath { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class PaginatedEnvelope
    {
        public List<AuditLogResponse> Items { get; set; } =new ();
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
}
