using BCrypt.Net;
using HalaqatBackend.DTOs.Profile;
using HalaqatBackend.Enums;
using HalaqatBackend.Repositories.Profile;
using HalaqatBackend.Repositories.RefreshTokens;
using HalaqatBackend.Utils;

namespace HalaqatBackend.Services.Profile
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public ProfileService(
            IProfileRepository profileRepository,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _profileRepository = profileRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<ProfileResponseDto> GetUserProfileAsync(string userId)
        {
            var user = await _profileRepository.GetUserProfileAsync(userId);
            
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            return new ProfileResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Role = user.Role,
                Gender = user.Gender
            };
        }

        public async Task<ProfileResponseDto> UpdateUserProfileAsync(string userId, UpdateProfileRequestDto request)
        {
            var user = await _profileRepository.GetUserProfileAsync(userId);
            
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            Gender? genderEnum = null;
            if (!string.IsNullOrWhiteSpace(request.Gender))
            {
                var normalizedGender = request.Gender.Trim().ToLower();
                if (normalizedGender != "male" && normalizedGender != "female")
                {
                    throw new ArgumentException("Gender must be either 'Male' or 'Female'");
                }
                genderEnum = normalizedGender == "male" ? Gender.Male : Gender.Female;
            }

            var updatedUser = await _profileRepository.UpdateUserProfileAsync(
                userId,
                request.FirstName,
                request.LastName,
                request.Phone,
                genderEnum);

            return new ProfileResponseDto
            {
                UserId = updatedUser.Id,
                Email = updatedUser.Email!,
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName,
                Phone = updatedUser.Phone,
                Role = updatedUser.Role,
                Gender = updatedUser.Gender
            };
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequestDto request)
        {
            var user = await _profileRepository.GetUserProfileAsync(userId);
            
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                throw new ArgumentException("Current password is incorrect");
            }

            PasswordValidator.ValidateOrThrow(request.NewPassword);

            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _profileRepository.UpdateUserPasswordAsync(userId, newPasswordHash);

            await _refreshTokenRepository.RevokeAllUserTokensAsync(userId);

            return true;
        }
    }
}
