using Microsoft.EntityFrameworkCore;
using SecureAuthDemo.Data;
using SecureAuthDemo.Models;

namespace SecureAuthDemo.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;
        public UserRepository(AppDbContext db) => _db = db;
        public async Task AddAsync(User user)
        {
            await _db.Users.AddAsync(user);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
