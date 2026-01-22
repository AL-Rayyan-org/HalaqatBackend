using HalaqatBackend.Models;
using HalaqatBackend.Repositories.AuditLogs;

namespace HalaqatBackend.Services.AuditLogs
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(IAuditLogRepository auditLogRepository, ILogger<AuditLogService> logger)
        {
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        public async Task LogActionAsync(string userId, string action, string entityName)
        {
            try
            {
                _logger.LogInformation("LogActionAsync called - UserId: {UserId}, Action: {Action}, EntityName: {EntityName}", 
                    userId, action, entityName);

                var auditLog = new AuditLog
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Date = DateTime.UtcNow,
                    Action = action,
                    EntityName = entityName
                };

                _logger.LogInformation("Calling repository to create audit log with Id: {Id}", auditLog.Id);

                await _auditLogRepository.CreateAsync(auditLog);
                
                _logger.LogInformation("Audit log created successfully in database with Id: {Id}", auditLog.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create audit log for user {UserId}, action {Action}", userId, action);
                throw;
            }
        }

        public async Task<IEnumerable<AuditLog>> GetUserLogsAsync(string userId, int limit = 100)
        {
            return await _auditLogRepository.GetByUserIdAsync(userId, limit);
        }

        public async Task<IEnumerable<AuditLog>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _auditLogRepository.GetByDateRangeAsync(startDate, endDate);
        }

        public async Task<IEnumerable<AuditLog>> GetAllLogsAsync(int limit = 1000)
        {
            return await _auditLogRepository.GetAllAsync(limit);
        }
    }
}
