using System.ComponentModel.DataAnnotations;

namespace SecureAuthDemo.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [MaxLength(50)]
        public string Username { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(255)]
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
