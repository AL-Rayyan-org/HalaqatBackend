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
        Task<IEnumerable<User>> GetAllActiveUsersAsync(string? searchText, Roles? role);
        Task<bool> UpdateUserRoleAsync(string userId, Roles role);
        Task<bool> DeactivateUserAsync(string userId);
        Task<bool> RemoveFromGroupTeachersAsync(string userId);
        Task<bool> UpdateUserPasswordAsync(string userId, string passwordHash);
    }
}
