using Dapper;
using HalaqatBackend.Data;
using HalaqatBackend.Models;

namespace HalaqatBackend.Repositories.RefreshTokens
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DapperContext _context;

        public RefreshTokenRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM refresh_tokens WHERE token = @Token";
            return await connection.QueryFirstOrDefaultAsync<RefreshToken>(sql, new { Token = token });
        }

        public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
        {
            using var connection = _context.CreateConnection();
            var sql = @"INSERT INTO refresh_tokens (id, user_id, token, expires_at, created_at)
                       VALUES (@Id, @UserId, @Token, @ExpiresAt, @CreatedAt)
                       RETURNING *";
            
            var createdToken = await connection.QuerySingleAsync<RefreshToken>(sql, refreshToken);
            return createdToken;
        }

        public async Task<bool> RevokeAsync(string token)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE refresh_tokens SET revoked_at = @RevokedAt WHERE token = @Token";
            var affectedRows = await connection.ExecuteAsync(sql, new { Token = token, RevokedAt = DateTime.UtcNow });
            return affectedRows > 0;
        }

        public async Task<bool> RevokeAllUserTokensAsync(string userId)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE refresh_tokens SET revoked_at = @RevokedAt WHERE user_id = @UserId AND revoked_at IS NULL";
            var affectedRows = await connection.ExecuteAsync(sql, new { UserId = userId, RevokedAt = DateTime.UtcNow });
            return affectedRows > 0;
        }
    }
}

