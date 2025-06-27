using MeraStore.Shared.Kernel.Exceptions.Core;
using MeraStore.Shared.Kernel.Exceptions.Helpers;
using MeraStore.Shared.Kernel.WebApi.Middleware;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MeraStore.Services.Cross.Cutting.Api.Middlewares;

/// <summary>
/// Application-specific error handling middleware that extends the shared base.
/// Override hook methods to inject custom handling logic.
/// </summary>
public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    : BaseErrorHandlingMiddleware(next, logger)
{
    /// <summary>
    /// Handles structured exceptions derived from <see cref="BaseAppException"/> and transforms them into
    /// RFC 7807-compliant <see cref="ProblemDetails"/> responses.
    /// Also logs the exception details using structured logging for diagnostics.
    /// </summary>
    /// <param name="context">HTTP context for the current request.</param>
    /// <param name="exception">The structured application exception instance.</param>
    /// <returns>A task that represents the asynchronous operation of writing the problem response.</returns>
    protected override Task HandleStructuredExceptionAsync(HttpContext context, BaseAppException exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)exception.StatusCode;

        var problemDetails = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Type = exception.EventCode ?? GetRequestEventCode(context),
            Title = "An error occurred while processing your request.",
            Detail = exception.Message,
            Instance = context.TraceIdentifier
        };

        problemDetails.Extensions["errorCode"] = exception.FullErrorCode;
        problemDetails.Extensions["category"] = exception.Category.ToString();
        problemDetails.Extensions["severity"] = exception.Severity.ToString();
        problemDetails.Extensions["service"] = ServiceCodeRegistry.GetKey(exception.ServiceIdentifier);
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        logger.LogError(exception, "Structured error occurred: {Message} | Code: {Code} | Category: {Category}",
            exception.Message, exception.FullErrorCode, exception.Category);

        return context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails, JsonSerializerSettings));
    }

}