using Dapper;
using HalaqatBackend.Data;
using HalaqatBackend.Models;
using HalaqatBackend.Utils;

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
                       VALUES (@Id, @FirstName, @LastName, @Email, @PasswordHash, @Phone, @IsActive, @RoleString, @CreatedAt)
                       RETURNING *";
            
            var parameters = new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PasswordHash,
                user.Phone,
                user.IsActive,
                RoleString = user.Role.ToString(),
                user.CreatedAt
            };
            
            var createdUser = await connection.QuerySingleAsync<User>(sql, parameters);
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

        public async Task<IEnumerable<User>> GetAllActiveUsersAsync(string? searchText, Roles? role)
        {
            using var connection = _context.CreateConnection();
            
            var parameters = new Dictionary<string, object>();
            var whereClauses = new List<string> { "is_active = true", "role IN ('Owner', 'Admin', 'Teacher')" };

            searchText = SearchHelper.NormalizeSearchText(searchText);
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var searchClause = SearchHelper.BuildSearchClause(
                    searchText, 
                    new[] { "first_name", "last_name", "email" }, 
                    parameters
                );
                whereClauses.Add(searchClause);
            }

            if (role.HasValue)
            {
                whereClauses.Add("role = @Role");
                parameters["Role"] = role.Value.ToString();
            }

            var whereClause = string.Join(" AND ", whereClauses);
            var sql = $"SELECT * FROM users WHERE {whereClause} ORDER BY created_at DESC";

            return await connection.QueryAsync<User>(sql, parameters);
        }

        public async Task<bool> UpdateUserRoleAsync(string userId, Roles role)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE users SET role = @Role WHERE id = @UserId";
            var affectedRows = await connection.ExecuteAsync(sql, new { UserId = userId, Role = role.ToString() });
            return affectedRows > 0;
        }

        public async Task<bool> DeactivateUserAsync(string userId)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE users SET is_active = false WHERE id = @UserId";
            var affectedRows = await connection.ExecuteAsync(sql, new { UserId = userId });
            return affectedRows > 0;
        }

        public async Task<bool> RemoveFromGroupTeachersAsync(string userId)
        {
            using var connection = _context.CreateConnection();
            var sql = "DELETE FROM group_teachers WHERE teacher_id = @UserId";
            await connection.ExecuteAsync(sql, new { UserId = userId });
            return true;
        }

        public async Task<bool> UpdateUserPasswordAsync(string userId, string passwordHash)
        {
            using var connection = _context.CreateConnection();
            var sql = "UPDATE users SET password_hash = @PasswordHash WHERE id = @UserId";
            var affectedRows = await connection.ExecuteAsync(sql, new { UserId = userId, PasswordHash = passwordHash });
            return affectedRows > 0;
        }
    }
}

