using HalaqatBackend.Models;

namespace HalaqatBackend.Repositories.Users
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(string id);
        Task<bool> EmailExistsAsync(string email);
    }
}
