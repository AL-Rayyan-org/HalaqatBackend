using Dapper;
using HalaqatBackend.Data;
using HalaqatBackend.Models;

namespace HalaqatBackend.Repositories.Profile
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly DapperContext _context;

        public ProfileRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserProfileAsync(string userId)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM users WHERE id = @UserId";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { UserId = userId });
        }

        public async Task<User> UpdateUserProfileAsync(string userId, string firstName, string lastName, string? phone)
        {
            using var connection = _context.CreateConnection();
            var sql = @"UPDATE users 
                       SET first_name = @FirstName, 
                           last_name = @LastName, 
                           phone = @Phone
                       WHERE id = @UserId
                       RETURNING *";
            
            var updatedUser = await connection.QuerySingleAsync<User>(sql, new 
            { 
                UserId = userId,
                FirstName = firstName, 
                LastName = lastName, 
                Phone = phone 
            });
            return updatedUser;
        }

        public async Task UpdateUserPasswordAsync(string userId, string passwordHash)
        {
            using var connection = _context.CreateConnection();
            var sql = @"UPDATE users 
                       SET password_hash = @PasswordHash
                       WHERE id = @UserId";
            
            await connection.ExecuteAsync(sql, new 
            { 
                UserId = userId,
                PasswordHash = passwordHash 
            });
        }
    }
}
