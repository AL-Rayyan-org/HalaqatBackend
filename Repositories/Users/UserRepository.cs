using Dapper;
using HalaqatBackend.Data;
using HalaqatBackend.Models;

namespace HalaqatBackend.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM users WHERE id = @Id";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT * FROM users WHERE email = @Email";
            return await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });
        }

        public async Task<User> CreateAsync(User user)
        {
            using var connection = _context.CreateConnection();
            var sql = @"INSERT INTO users (id, first_name, last_name, email, password_hash, phone, is_active, role, created_at)
                       VALUES (@Id, @FirstName, @LastName, @Email, @PasswordHash, @Phone, @IsActive, @Role, @CreatedAt)
                       RETURNING *";
            
            var createdUser = await connection.QuerySingleAsync<User>(sql, user);
            return createdUser;
        }

        public async Task<User> UpdateAsync(User user)
        {
            using var connection = _context.CreateConnection();
            var sql = @"UPDATE users 
                       SET first_name = @FirstName, 
                           last_name = @LastName, 
                           email = @Email, 
                           password_hash = @PasswordHash, 
                           phone = @Phone, 
                           is_active = @IsActive, 
                           role = @Role
                       WHERE id = @Id
                       RETURNING *";
            
            var updatedUser = await connection.QuerySingleAsync<User>(sql, user);
            return updatedUser;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM users WHERE id = @Id";
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            using var connection = _context.CreateConnection();
            var sql = "SELECT COUNT(1) FROM users WHERE email = @Email";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email });
            return count > 0;
        }
    }
}

