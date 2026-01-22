using HalaqatBackend.Models;

namespace HalaqatBackend.Services.AuditLogs
{
    public interface IAuditLogService
    {
        Task LogActionAsync(string userId, string action, string entityName);
        Task<IEnumerable<AuditLog>> GetUserLogsAsync(string userId, int limit = 100);
        Task<IEnumerable<AuditLog>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<AuditLog>> GetAllLogsAsync(int limit = 1000);
    }
}
