using HalaqatBackend.DTOs.Users;

namespace HalaqatBackend.Services.Users
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDto>> GetAllActiveUsersAsync(string? searchText, string? role);
        Task<UserResponseDto> CreateUserAsync(CreateUserRequestDto request);
        Task<UserResponseDto> ChangeUserRoleAsync(string currentUserId, string userId, string newRole);
        Task<bool> DeactivateUserAsync(string userId);
        Task<bool> ChangeUserPasswordAsync(string userId, string newPassword);
    }
}
