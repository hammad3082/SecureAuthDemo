using Microsoft.EntityFrameworkCore;
using SecureAuthDemo.Models;

namespace SecureAuthDemo.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
                
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique()
                .IncludeProperties(u => u.PasswordHash)
                .HasDatabaseName("IX_Users_Email_Login_Covering");

            modelBuilder.Entity<AuditLog>(entity =>
            {
                // High-performance index for filtering user history by time
                entity.HasIndex(e => new { e.UserId, e.Timestamp })
                      .HasDatabaseName("IX_AuditLogs_UserId_Timestamp");
            });
        }
    }
}
