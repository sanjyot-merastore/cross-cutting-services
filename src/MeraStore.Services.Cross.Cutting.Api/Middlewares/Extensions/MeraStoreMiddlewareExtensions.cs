using MeraStore.Shared.Kernel.Logging;

namespace MeraStore.Services.Cross.Cutting.Api.Middlewares.Extensions;

/// <summary>
/// Extension methods to register standard MeraStore middleware components.
/// </summary>
public static class MeraStoreMiddlewareExtensions
{
    /// <summary>
    /// Adds MeraStore tracing middleware.
    /// Enriches requests with correlation ID, request ID, and transaction ID.
    /// </summary>
    public static IApplicationBuilder UseMeraStoreTracing(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TracingMiddleware>(Constants.ServiceName);
    }

    /// <summary>
    /// Adds MeraStore error handling middleware.
    /// Handles exceptions and returns standardized error responses.
    /// </summary>
    public static IApplicationBuilder UseMeraStoreErrorHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }

    /// <summary>
    /// Adds MeraStore API logging middleware.
    /// Logs request/response payloads with masking support.
    /// </summary>
    public static IApplicationBuilder UseMeraStoreLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LoggingMiddleware>();
    }

}