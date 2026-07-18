using SecureAuthDemo.Enums;
using System.ComponentModel.DataAnnotations;

namespace SecureAuthDemo.Entities
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

        [MaxLength(15)]
        public AuthProvider LoginProvider { get; set; } = AuthProvider.Local;
        public bool IsTwoFactorEnabled { get; set; } = false;
    }
}
