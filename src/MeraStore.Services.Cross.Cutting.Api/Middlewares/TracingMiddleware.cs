using MeraStore.Shared.Kernel.Context;
using MeraStore.Shared.Kernel.WebApi.Middleware;

namespace MeraStore.Services.Cross.Cutting.Api.Middlewares;

/// <summary>
/// Middleware that enriches incoming and outgoing HTTP requests
/// with tracing metadata for observability and diagnostics.
/// Inherits from <see cref="BaseAppContextMiddleware"/> to reuse core logic.
/// </summary>
public class TracingMiddleware(RequestDelegate next, string serviceName)
    : BaseAppContextMiddleware(next, serviceName)
{
    /// <summary>
    /// Enriches request headers with trace-related metadata before passing it down the pipeline.
    /// Override to customize header propagation or add new headers.
    /// </summary>
    /// <param name="context">HTTP context for the current request.</param>
    /// <param name="appContext">AppContext instance containing trace data.</param>
    protected override void EnrichRequestHeaders(HttpContext context, AppContextBase appContext)
    {
        if (ShouldTrace(context))
        {
            base.EnrichRequestHeaders(context, appContext);
        }
    }

    

    /// <summary>
    /// Enriches response headers with trace identifiers for client-side tracking.
    /// Override to customize header exposure.
    /// </summary>
    /// <param name="context">HTTP context for the current response.</param>
    /// <param name="appContext">AppContext instance containing trace data.</param>
    protected override void EnrichResponseHeaders(HttpContext context, AppContextBase appContext)
    {
        base.EnrichResponseHeaders(context, appContext);
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
