using HalaqatBackend.Models;

namespace HalaqatBackend.Repositories.AuditLogs
{
    public interface IAuditLogRepository
    {
        Task<AuditLog> CreateAsync(AuditLog auditLog);
        Task<IEnumerable<AuditLog>> GetByUserIdAsync(string userId, int limit = 100);
        Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<AuditLog>> GetAllAsync(int limit = 1000);
    }
}
