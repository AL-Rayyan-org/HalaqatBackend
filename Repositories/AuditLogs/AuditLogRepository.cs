using Dapper;
using HalaqatBackend.Data;
using HalaqatBackend.Models;

namespace HalaqatBackend.Repositories.AuditLogs
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly DapperContext _context;

        public AuditLogRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<AuditLog> CreateAsync(AuditLog auditLog)
        {
            using var connection = _context.CreateConnection();
            var sql = @"INSERT INTO audit_logs (id, user_id, date, action, entity_name)
                       VALUES (@Id, @UserId, @Date, @Action, @EntityName)
                       RETURNING *";
            
            var createdLog = await connection.QuerySingleAsync<AuditLog>(sql, auditLog);
            return createdLog;
        }

        public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId, int limit = 100)
        {
            using var connection = _context.CreateConnection();
            var sql = @"SELECT * FROM audit_logs 
                       WHERE user_id = @UserId 
                       ORDER BY date DESC 
                       LIMIT @Limit";
            
            return await connection.QueryAsync<AuditLog>(sql, new { UserId = userId, Limit = limit });
        }

        public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            using var connection = _context.CreateConnection();
            var sql = @"SELECT * FROM audit_logs 
                       WHERE date >= @StartDate AND date <= @EndDate 
                       ORDER BY date DESC";
            
            return await connection.QueryAsync<AuditLog>(sql, new { StartDate = startDate, EndDate = endDate });
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync(int limit = 1000)
        {
            using var connection = _context.CreateConnection();
            var sql = @"SELECT * FROM audit_logs 
                       ORDER BY date DESC 
                       LIMIT @Limit";
            
            return await connection.QueryAsync<AuditLog>(sql, new { Limit = limit });
        }
    }
}
