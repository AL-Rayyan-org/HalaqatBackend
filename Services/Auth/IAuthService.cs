using HalaqatBackend.DTOs.Auth;

namespace HalaqatBackend.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequestDto request);
    }
}
