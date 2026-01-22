using HalaqatBackend.DTOs;
using HalaqatBackend.Services.AuditLogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HalaqatBackend.Controllers
{
    [ApiController]
    [Route("audit-logs")]
    [Authorize(Roles = "Owner,Admin")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogsController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllLogs([FromQuery] int limit = 1000)
        {
            try
            {
                var logs = await _auditLogService.GetAllLogsAsync(limit);
                return Ok(ApiResponse<object>.SuccessResponse(logs, "Audit logs retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message, 500));
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserLogs(string userId, [FromQuery] int limit = 100)
        {
            try
            {
                var logs = await _auditLogService.GetUserLogsAsync(userId, limit);
                return Ok(ApiResponse<object>.SuccessResponse(logs, "User audit logs retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message, 500));
            }
        }

        [HttpGet("date-range")]
        public async Task<IActionResult> GetLogsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                {
                    return BadRequest(ApiResponse<object>.ErrorResponse("Start date must be before end date", 400));
                }

                var logs = await _auditLogService.GetLogsByDateRangeAsync(startDate, endDate);
                return Ok(ApiResponse<object>.SuccessResponse(logs, "Audit logs retrieved successfully", 200));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.ErrorResponse(ex.Message, 500));
            }
        }
    }
}
