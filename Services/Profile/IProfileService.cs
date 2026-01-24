using HalaqatBackend.DTOs.Profile;

namespace HalaqatBackend.Services.Profile
{
    public interface IProfileService
    {
        Task<ProfileResponseDto> GetUserProfileAsync(string userId);
        Task<ProfileResponseDto> UpdateUserProfileAsync(string userId, UpdateProfileRequestDto request);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequestDto request);
    }
}
