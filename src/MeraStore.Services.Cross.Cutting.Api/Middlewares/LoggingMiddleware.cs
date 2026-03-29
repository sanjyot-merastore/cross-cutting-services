using MeraStore.Services.Cross.Cutting.Api.Filters;
using MeraStore.Shared.Kernel.Logging.Interfaces;
using MeraStore.Shared.Kernel.Logging.Loggers;
using MeraStore.Shared.Kernel.WebApi.Middlewares;

namespace MeraStore.Services.Cross.Cutting.Api.Middlewares;

/// <summary>
/// Custom middleware for logging HTTP request and response details in the Sample service.
/// Inherits from <see cref="BaseLoggingMiddleware"/> to allow for structured logging,
/// request/response masking, and performance tracking.
/// </summary>
/// <param name="next">The next delegate/middleware in the HTTP pipeline.</param>
/// <param name="maskingFilter">The masking filter used to sanitize sensitive data from payloads.</param>
public class LoggingMiddleware(RequestDelegate next, IMaskingFilter maskingFilter)
    : BaseLoggingMiddleware(next, maskingFilter)
{
    /// <summary>
    /// Enriches the <see cref="ApiLog"/> instance with additional context-specific fields.
    /// Override this method to add service-specific metadata (e.g., user roles, headers, etc.).
    /// </summary>
    /// <param name="apiLog">The structured API log to enrich.</param>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A completed <see cref="Task"/>.</returns>
    protected override Task EnrichApiLogAsync(ApiLog apiLog, HttpContext context)
    {
        // Example: apiLog.AdditionalData["UserId"] = context.User.Identity?.Name;
        if(ShouldTrace(context))
        {
            base.EnrichApiLogAsync(apiLog, context);
            
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds masking filters to the API log for sanitizing sensitive fields.
    /// You can override this method to apply custom or multiple masking filters.
    /// </summary>
    /// <param name="apiLog">The API log where masking filters will be applied.</param>
    protected override void AddMaskingFilters(ApiLog apiLog)
    {
        base.AddMaskingFilters(apiLog);
    }

    private bool ShouldTrace(HttpContext context)
    {
        var path = context.Request.Path.Value;

        if (string.IsNullOrWhiteSpace(path))
            return false;

        // Normalize path (e.g., "/favicon.ico" -> "favicon.ico")
        path = path.Trim().ToLowerInvariant();

        // Skip root, favicon, and static junk
        if (path == "/" || path == "/favicon.ico" || path.StartsWith("/swagger") || path.StartsWith("/health"))
            return false;

        // Only trace if the path starts with /api/
        return path.StartsWith("/api/");
    }
}
