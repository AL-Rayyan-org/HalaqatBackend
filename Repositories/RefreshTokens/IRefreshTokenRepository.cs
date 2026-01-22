using HalaqatBackend.Models;

namespace HalaqatBackend.Repositories.RefreshTokens
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
        Task<bool> RevokeAsync(string token);
        Task<bool> RevokeAllUserTokensAsync(string userId);
    }
}
