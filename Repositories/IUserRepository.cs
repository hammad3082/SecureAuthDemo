using SecureAuthDemo.Models;

namespace SecureAuthDemo.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByIdAsync(int userId);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}
