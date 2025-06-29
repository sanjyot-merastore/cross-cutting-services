using System.Net;
using MeraStore.Shared.Kernel.Exceptions;
using MeraStore.Shared.Kernel.Exceptions.Codes.Events;
using MeraStore.Shared.Kernel.Exceptions.Codes.Http;
using MeraStore.Shared.Kernel.Exceptions.Codes.Services;
using MeraStore.Shared.Kernel.Exceptions.Core;
using MeraStore.Shared.Kernel.Logging.Attributes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ValidationException = FluentValidation.ValidationException;

namespace MeraStore.Services.Cross.Cutting.Api.Middlewares;

/// <summary>
/// Middleware for centralized exception handling across HTTP requests.
/// Captures and processes different exception types—including FluentValidation and custom application exceptions—
/// and returns standardized <see cref="ProblemDetails"/> or <see cref="ValidationProblemDetails"/> responses.
/// </summary>
/// <remarks>
/// This middleware also logs detailed error information using structured logging,
/// enriches responses with traceability metadata (e.g., <c>traceId</c>, <c>errorCode</c>, etc.),
/// and ensures consistency across microservices.
/// </remarks>
/// <param name="next">The next middleware component in the pipeline.</param>
/// <param name="logger">The logger instance used for writing error logs.</param>
public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    /// <summary>
    /// Configured JSON serializer settings for clean and consistent error response formatting.
    /// </summary>
    protected readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        DateFormatHandling = DateFormatHandling.IsoDateFormat,
        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
        Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
    };

    /// <summary>
    /// Core middleware logic that wraps the HTTP request pipeline in a try-catch block
    /// and handles different exception types gracefully.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    public virtual async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (BaseAppException ex)
        {
            await HandleStructuredExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            var wrapped = new CommonException(
                EventCodeRegistry.GetCode(Constants.EventCodes.InternalServerError),
                ErrorCodeRegistry.GetCode(Constants.ErrorCodes.InternalServerError),
                HttpStatusCode.InternalServerError,
                "An unexpected server error occurred.",
                innerException: ex,
                category: ExceptionCategory.Operational,
                severity: ExceptionSeverity.Critical
            );

            await HandleStructuredExceptionAsync(context, wrapped);
        }
    }

    /// <summary>
    /// Handles <see cref="ValidationException"/>s by returning a <see cref="ValidationProblemDetails"/> response.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The thrown <see cref="ValidationException"/>.</param>
    protected virtual async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        logger.LogError(exception, "Validation error occurred: {@ValidationErrors}",
            exception.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        var validationErrors = exception.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        var problemDetails = new ValidationProblemDetails(validationErrors)
        {
            Type = GetRequestEventCode(context),
            Status = context.Response.StatusCode,
            Title = "Validation failed.",
            Instance = context.TraceIdentifier
        };

        await context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails, JsonSerializerSettings));
    }

    /// <summary>
    /// Handles all <see cref="BaseAppException"/> types by returning structured <see cref="ProblemDetails"/>.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The custom application exception.</param>
    protected virtual Task HandleStructuredExceptionAsync(HttpContext context, BaseAppException exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)exception.StatusCode;

        var problemDetails = new ProblemDetails
        {
            Status = context.Response.StatusCode,
            Type = EventCodeRegistry.GetKey(exception.EventCode) ?? EventCodeRegistry.GetKey(GetRequestEventCode(context)),
            Title = "An error occurred while processing your request.",
            Detail = exception.Message,
            Instance = context.TraceIdentifier
        };

        // Structured extensions
        problemDetails.Extensions["errorCode"] = exception.FullErrorCode;
        problemDetails.Extensions["category"] = exception.Category.ToString();
        problemDetails.Extensions["severity"] = exception.Severity.ToString();

        if (!string.IsNullOrWhiteSpace(exception.ServiceIdentifier) &&
            exception.ServiceIdentifier.All(char.IsDigit))
        {
            problemDetails.Extensions["service"] = ServiceCodeRegistry.GetKey(exception.ServiceIdentifier);
        }
        else
        {
            problemDetails.Extensions["service"] = exception.ServiceIdentifier;
        }

        // Optionally add correlation ID
        if (context.Request.Headers.TryGetValue("x-correlation-id", out var correlationId))
            problemDetails.Extensions["correlationId"] = correlationId.ToString();

        if (exception.InnerException is not null)
        {
            problemDetails.Extensions["innerException"] = new
            {
                message = exception.InnerException.Message,
                type = exception.InnerException.GetType().FullName,
#if DEBUG
                stackTrace = exception.InnerException.StackTrace
#endif
            };
        }

        logger.LogError(exception, "Structured error occurred: {Message} | Code: {Code} | Category: {Category}",
            exception.Message, exception.FullErrorCode, exception.Category);

        return context.Response.WriteAsync(JsonConvert.SerializeObject(problemDetails, JsonSerializerSettings));
    }


    /// <summary>
    /// Retrieves the event code metadata associated with the current request endpoint.
    /// Falls back to <c>InternalServerError</c> if none is found.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>The resolved event code string.</returns>
    protected virtual string GetRequestEventCode(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var code = endpoint?.Metadata.GetMetadata<EventCodeAttribute>()?.EventCode;

        return string.IsNullOrWhiteSpace(code)
            ? Constants.EventCodes.InternalServerError
            : code;
    }
}
