using HalaqatBackend.Services.AuditLogs;
using System.Security.Claims;

namespace HalaqatBackend.Middleware
{
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditLogMiddleware> _logger;

        public AuditLogMiddleware(RequestDelegate next, ILogger<AuditLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuditLogService auditLogService)
        {
            _logger.LogInformation("AuditLogMiddleware invoked for {Method} {Path}", context.Request.Method, context.Request.Path);

            await _next(context);

            if (ShouldLogRequest(context))
            {
                _logger.LogInformation("Request should be logged: {Method} {Path} - Status: {StatusCode}", 
                    context.Request.Method, context.Request.Path, context.Response.StatusCode);
                await LogRequestAsync(context, auditLogService);
            }
            else
            {
                _logger.LogInformation("Request skipped from logging: {Method} {Path} - Status: {StatusCode}", 
                    context.Request.Method, context.Request.Path, context.Response.StatusCode);
            }
        }

        private bool ShouldLogRequest(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? string.Empty;
            var method = context.Request.Method.ToUpper();

            // Skip OpenAPI/Swagger endpoints
            if (path.Contains("/openapi") || path.Contains("/swagger"))
                return false;

            // Skip health check endpoints if any
            if (path.Contains("/health"))
                return false;

            // Only log state-changing operations (not GET requests)
            // You can modify this to log all requests if needed
            var isStatefulOperation = method == "POST" || method == "PUT" || method == "DELETE" || method == "PATCH";

            // Only log successful requests (2xx status codes)
            var isSuccessful = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300;

            return isStatefulOperation && isSuccessful;
        }

        private async Task LogRequestAsync(HttpContext context, IAuditLogService auditLogService)
        {
            try
            {
                // Extract user ID from JWT claims
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                // If no user is authenticated, use "Anonymous" or skip logging
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogDebug("Skipping audit log - no authenticated user");
                    return;
                }

                var method = context.Request.Method;
                var path = context.Request.Path.Value ?? string.Empty;
                
                // Extract entity name from route
                var entityName = ExtractEntityNameFromPath(path);
                
                // Build action description
                var action = BuildActionDescription(method, path);

                await auditLogService.LogActionAsync(userId, action, entityName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log audit entry");
            }
        }

        private string ExtractEntityNameFromPath(string path)
        {
            // Remove leading/trailing slashes and split
            var segments = path.Trim('/').Split('/');
            
            // Skip "v1" or "api" prefixes
            var relevantSegments = segments.Where(s => 
                !s.Equals("v1", StringComparison.OrdinalIgnoreCase) && 
                !s.Equals("api", StringComparison.OrdinalIgnoreCase)).ToList();
            
            if (relevantSegments.Count > 0)
            {
                // Return the controller/entity name (first relevant segment)
                return relevantSegments[0];
            }

            return "Unknown";
        }

        private string BuildActionDescription(string method, string path)
        {
            var action = method switch
            {
                "POST" => "Created",
                "PUT" => "Updated",
                "DELETE" => "Deleted",
                "PATCH" => "Modified",
                _ => "Action"
            };

            return $"{action} {path}";
        }
    }
}
