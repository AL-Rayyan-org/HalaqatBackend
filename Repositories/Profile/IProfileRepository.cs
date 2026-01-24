using HalaqatBackend.Models;

namespace HalaqatBackend.Repositories.Profile
{
    public interface IProfileRepository
    {
        Task<User?> GetUserProfileAsync(string userId);
        Task<User> UpdateUserProfileAsync(string userId, string firstName, string lastName, string? phone);
        Task UpdateUserPasswordAsync(string userId, string passwordHash);
    }
}
