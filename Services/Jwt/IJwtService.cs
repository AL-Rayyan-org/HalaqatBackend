using System.Security.Claims;

namespace HalaqatBackend.Services.Jwt
{
    public interface IJwtService
    {
        string GenerateAccessToken(string userId, string email, Roles role);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
    }
}
